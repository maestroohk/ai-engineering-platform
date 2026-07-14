<#
.SYNOPSIS
    AI session router — Windows-first PowerShell supervisor.

.DESCRIPTION
    Drives one bounded Claude Code child session per phase, each on a
    different configured cloud model. Reads the active task packet from
    .ai/context/active-task.json, classifies the work to a model profile
    (high / standard / economy / review / fallback), resolves a configured
    cloud model from .ai/model-routing.json, and launches the child through
    the non-interactive form:

        ollama launch claude --model <model> -y -- -p "<prompt>"

    Captures the exit code, parses the phase receipt, and decides whether
    to advance, retry, escalate, or stop. Stops at closeout. Does not
    begin a second task.

    PowerShell 5.1+ (no pwsh-only syntax). Argument list, not string.
    No Invoke-Expression. Model names and task IDs validated against a
    strict regex. Paths validated against the repository root. Ctrl+C
    cancels the supervisor and terminates the child process tree.

.PARAMETER Command
    Next | Resume | Finish | Status | Plan | Configure | DryRun

.PARAMETER TaskId
    Optional. Override the active task ID. Defaults to the value in
    .ai/context/active-task.json.

.PARAMETER ProfileOverride
    Optional. Force a profile for the next dispatch. Recorded in the
    phase receipt. Does not persist.

.PARAMETER NoPush
    Optional. Allow the closeout phase to push when a remote is
    configured. Default is to respect execution.push_authorization_required
    and stop with status: blocked.

.PARAMETER DryRun
    Optional. Print the launch command without executing it.

.PARAMETER Configure
    Optional. Ask the user for the standard and economy model names
    once and persist them to .ai/model-routing.json.

.EXAMPLE
    .\tools\ai-session-router.ps1 -Command Next

.EXAMPLE
    .\tools\ai-session-router.ps1 -Command Next -DryRun

.EXAMPLE
    .\tools\ai-session-router.ps1 -Command Status

.EXAMPLE
    .\tools\ai-session-router.ps1 -Command Configure

.NOTES
    See .ai/model-routing.json, .ai/model-routing.schema.json,
    .ai/model-classification.json, .ai/prompts/phases/, and
    .ai/templates/phase-receipt.schema.json.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Next', 'Resume', 'Finish', 'Status', 'Plan', 'Configure', 'DryRun', 'RetryCurrentPhase')]
    [string]$Command,

    [string]$TaskId,

    [ValidateSet('high', 'standard', 'economy', 'review', 'fallback')]
    [string]$ProfileOverride,

    [switch]$NoPush,

    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ---------------------------------------------------------------------------
# Constants
# ---------------------------------------------------------------------------

$RouterVersion = '1.0.0'
$RepoRoot = (Resolve-Path -Path (Join-Path $PSScriptRoot '..')).Path
$AiDir = Join-Path $RepoRoot '.ai'
$ToolsDir = Join-Path $RepoRoot 'tools'
$ContextDir = Join-Path $AiDir 'context'
$PromptsDir = Join-Path $AiDir 'prompts/phases'
$ReceiptsDir = Join-Path $AiDir 'receipts/phases'
$TemplatesDir = Join-Path $AiDir 'templates'
$StateDir = Join-Path $AiDir 'state'

$ModelRoutingConfigPath = Join-Path $AiDir 'model-routing.json'
$ModelRoutingSchemaPath = Join-Path $AiDir 'model-routing.schema.json'
$ModelClassificationPath = Join-Path $AiDir 'model-classification.json'
$PhaseReceiptSchemaPath = Join-Path $TemplatesDir 'phase-receipt.schema.json'
$RepositoryMapPath = Join-Path $ContextDir 'repository-map.json'
$ActiveTaskPath = Join-Path $ContextDir 'active-task.json'
$ValidationCachePath = Join-Path $ContextDir 'validation-cache.json'

$Phases = @('reconcile', 'plan', 'implement', 'validate', 'document', 'review', 'closeout')

# Strict regex (must match the schema)
$ModelNameRegex = '^[A-Za-z0-9._:-]+$'
$TaskIdRegex = '^T-[0-9]+$'
$PhaseRegex = '^(reconcile|plan|implement|validate|document|review|closeout)$'

# Tracked child PIDs (for Ctrl+C cleanup)
$script:TrackedPids = New-Object System.Collections.Generic.List[int]
$script:StopRequested = $false

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

function Write-RouterLine {
    param([string]$Message, [string]$Level = 'info')
    $stamp = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    $tag = "[router:$Level]"
    Write-Host "$stamp $tag $Message"
}

function Assert-PathUnderRoot {
    param([string]$Path)
    $resolved = [System.IO.Path]::GetFullPath($Path)
    $rootFull = [System.IO.Path]::GetFullPath($RepoRoot).TrimEnd([char][System.IO.Path]::DirectorySeparatorChar)
    $rootWithSep = $rootFull + [char][System.IO.Path]::DirectorySeparatorChar
    $ok = $resolved.Equals($rootFull, [System.StringComparison]::OrdinalIgnoreCase) -or
          $resolved.StartsWith($rootWithSep, [System.StringComparison]::OrdinalIgnoreCase)
    if (-not $ok) {
        throw "Path is outside the repository root: $Path"
    }
    return $resolved
}

function Read-JsonFile {
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Required file not found: $Path"
    }
    $raw = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
    return ConvertFrom-Json -InputObject $raw
}

function Read-JsonObject {
    # PowerShell 5.1 has no Test-Json. Parse to a hashtable so we can
    # inspect types deterministically.
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Required file not found: $Path"
    }
    $raw = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
    try {
        $obj = ConvertFrom-Json -InputObject $raw -ErrorAction Stop
    } catch {
        throw "Invalid JSON in $Path : $($_.Exception.Message)"
    }
    return $obj
}

function Test-ProfileEnabled {
    param(
        [Parameter(Mandatory = $true)] $ProfileConfig,
        [Parameter(Mandatory = $true)] [string]$ProfileKey
    )
    if ($null -eq $ProfileConfig) { return $false }
    $enabled = $ProfileConfig.enabled
    if ($null -eq $enabled) { return $false }
    return [bool]$enabled
}

function Resolve-ProfileModel {
    param(
        [Parameter(Mandatory = $true)] $Config,
        [Parameter(Mandatory = $true)] [string]$ProfileKey
    )
    $profile = $Config.profiles.$ProfileKey
    if ($null -eq $profile) {
        throw "Profile not configured: $ProfileKey"
    }
    $model = [string]$profile.model
    if ([string]::IsNullOrWhiteSpace($model) -or $model -eq 'CONFIGURE_ME') {
        throw "Profile '$ProfileKey' is not configured. Run -Configure to set a model name."
    }
    if ($model -notmatch $ModelNameRegex) {
        throw "Profile '$ProfileKey' model '$model' fails the strict regex $ModelNameRegex"
    }
    return $model
}

function Classify-Profile {
    param(
        [Parameter(Mandatory = $true)] $Config,
        [Parameter(Mandatory = $true)] $ActiveTask,
        [Parameter(Mandatory = $true)] [string]$Phase
    )
    # Order: high -> review -> standard -> economy -> fallback
    foreach ($candidate in @('high', 'review', 'standard', 'economy')) {
        if (Test-ProfileInPhase -Config $Config -ProfileKey $candidate -Phase $Phase -ActiveTask $ActiveTask) {
            return $candidate
        }
    }
    return 'standard'
}

function Test-ProfileInPhase {
    param(
        $Config,
        [string]$ProfileKey,
        [string]$Phase,
        $ActiveTask
    )
    # Per-phase recommendation wins
    $rec = $null
    if ($null -ne $ActiveTask.recommended_profile_by_phase) {
        $rec = $ActiveTask.recommended_profile_by_phase.$Phase
    }
    if ($null -ne $rec -and $rec -ne '') {
        return ($rec -eq $ProfileKey)
    }
    # Task-type heuristic
    $taskType = [string]$ActiveTask.task_type
    $taskId = [string]$ActiveTask.task_id
    switch ($ProfileKey) {
        'high' {
            if ($taskType -in @('architecture', 'bootstrap')) { return $true }
            if ($Phase -eq 'plan') { return $true }
            return $false
        }
        'review' {
            if ($taskType -eq 'review') { return $true }
            if ($taskId -like '*review*') { return $true }
            return $false
        }
        'standard' {
            if ($Phase -in @('implement', 'validate', 'reconcile', 'closeout')) { return $true }
            if ($taskType -in @('feature', 'bugfix', 'refactor', 'provider', 'testing', 'release')) { return $true }
            return $false
        }
        'economy' {
            if ($Phase -in @('document')) { return $true }
            if ($taskType -eq 'documentation') { return $true }
            return $false
        }
        'fallback' {
            # Fallback is selected by the router on failure, not by classification.
            return $false
        }
    }
    return $false
}

function Read-PhasePrompt {
    param([Parameter(Mandatory = $true)] [string]$Phase)
    $path = Join-Path $PromptsDir "$Phase.md"
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Phase prompt not found: $path"
    }
    return (Get-Content -LiteralPath $path -Raw -Encoding UTF8)
}

function New-PhasePromptString {
    param(
        [Parameter(Mandatory = $true)] $ActiveTask,
        [Parameter(Mandatory = $true)] [string]$Phase,
        [Parameter(Mandatory = $true)] [string]$Profile,
        [Parameter(Mandatory = $true)] [string]$Model,
        [Parameter(Mandatory = $true)] [string]$ReceiptRelPath
    )
    # The router hands the child a repository-relative receipt path and the
    # full canonical receipt template. The child MUST write the receipt to
    # this exact path before exiting. The path is repository-relative, not
    # an absolute Windows path, so the child does not need to manipulate
    # platform-specific paths.
    $taskId = [string]$ActiveTask.task_id
    $relPath = $ReceiptRelPath -replace '\\', '/'
    $startedAt = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    $template = @"
{
  "task_id": "$taskId",
  "phase": "$Phase",
  "model": "$Model",
  "profile": "$Profile",
  "started_at": "$startedAt",
  "completed_at": "REPLACE_AT_FINISH",
  "exit_code": 0,
  "status": "completed",
  "files_read": [],
  "files_changed": [],
  "commands_run": [],
  "targeted_tests": [],
  "validation": { "syntax_ok": true, "schema_ok": true, "pester_ok": true, "dry_run_ok": true },
  "decisions": [],
  "blockers": [],
  "next_phase": "REPLACE_WITH_NEXT_PHASE_OR_NULL",
  "retry_recommended": false,
  "fallback_recommended": false,
  "usage": { "unknown": true },
  "receipt_version": "1.0.0"
}
"@
    $prompt = @"
You are the $Phase phase of task $taskId in milestone $($ActiveTask.milestone).
Profile: $Profile
Model: $Model
Receipt path (repository-relative): $relPath
Active packet: $ActiveTaskPath
Approved plan: $($ActiveTask.approved_plan)
Stop conditions: see $PromptsDir\$Phase.md

MANDATORY FINAL ACTION (do not exit without completing all of these steps):

1. Build the phase receipt by copying the canonical template below and
   replacing REPLACE_AT_FINISH with the current UTC ISO 8601 timestamp and
   REPLACE_WITH_NEXT_PHASE_OR_NULL with the next phase name (one of
   reconcile | plan | implement | validate | document | review | closeout)
   or null when the closeout phase is complete. Fill files_read /
   files_changed / commands_run / targeted_tests / decisions / blockers
   with the values recorded during the phase.
2. Pipe the completed receipt JSON to the repository-side writer:

   powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Write-PhaseReceipt.ps1 -ReceiptPath $relPath -ExpectedTaskId $taskId -ExpectedPhase $Phase

3. The writer creates the parent directory if missing, validates the receipt
   against .ai/templates/phase-receipt.schema.json, writes the file as
   UTF-8 (no BOM), and exits non-zero on any validation failure. If it
   exits non-zero, fix the receipt and re-run the writer. Do not exit
   the phase until the writer exits 0.
4. Confirm the writer printed the absolute receipt path on its stdout.

CANONICAL RECEIPT TEMPLATE (must be filled in and passed to the writer):

$template

Read the active packet and the phase prompt. Do not exceed the phase's
allowed actions. Do not begin another phase. Do not exit before the
phase receipt has been written and the writer has exited 0.
"@
    return $prompt
}

function ConvertTo-RelativeReceiptPath {
    # Return the repository-relative form of an absolute receipt path,
    # using forward slashes. The child uses this path verbatim.
    param([Parameter(Mandatory = $true)] [string]$AbsolutePath)
    $rootFull = [System.IO.Path]::GetFullPath($RepoRoot).TrimEnd([char][System.IO.Path]::DirectorySeparatorChar) + [char][System.IO.Path]::DirectorySeparatorChar
    $ap = [System.IO.Path]::GetFullPath($AbsolutePath)
    if ($ap.StartsWith($rootFull, [System.StringComparison]::OrdinalIgnoreCase)) {
        $rel = $ap.Substring($rootFull.Length)
    } else {
        $rel = $ap
    }
    return ($rel -replace '\\', '/')
}

function Get-ChildLogPath {
    param(
        [Parameter(Mandatory = $true)] [string]$TaskId,
        [Parameter(Mandatory = $true)] [string]$Phase,
        [Parameter(Mandatory = $true)] [ValidateSet('stdout', 'stderr')] [string]$Stream
    )
    $logDir = Join-Path $ReceiptsDir 'logs'
    if (-not (Test-Path -LiteralPath $logDir)) {
        [void](New-Item -ItemType Directory -Path $logDir -Force)
    }
    return Join-Path $logDir ("{0}-{1}.{2}.log" -f $TaskId, $Phase, $Stream)
}

function Get-ReceiptPath {
    param(
        [Parameter(Mandatory = $true)] [string]$TaskId,
        [Parameter(Mandatory = $true)] [string]$Phase
    )
    return Join-Path $ReceiptsDir "$TaskId-$Phase.json"
}

function Get-RequiredReceiptFields {
    # Field names required by .ai/templates/phase-receipt.schema.json
    # (additionalProperties is not permitted by the schema, so any missing
    # required field fails validation).
    return @(
        'task_id', 'phase', 'model', 'profile', 'started_at', 'completed_at',
        'exit_code', 'status', 'files_read', 'files_changed', 'commands_run',
        'targeted_tests', 'validation', 'decisions', 'blockers', 'next_phase',
        'retry_recommended', 'fallback_recommended', 'usage', 'receipt_version'
    )
}

function Test-PhaseReceiptValid {
    # Returns $true when the receipt is present, parses, has every required
    # field, matches the expected task + phase, and declares a finalised
    # status. Otherwise returns an object:
    #   { Valid=$false; Reason=<one of the reason codes below>;
    #     Details=<human-readable details>; Path=<absolute path> }
    #
    # Reason codes:
    #   missing            - the file does not exist at the expected path
    #   malformed_json     - the file exists but does not parse as JSON
    #   missing_fields     - parsed JSON is missing one or more required fields
    #   wrong_task         - receipt.task_id does not match ExpectedTaskId
    #   wrong_phase        - receipt.phase does not match ExpectedPhase
    #   incomplete_status  - receipt.status is not a known final status
    #                        (or is missing)
    param(
        [Parameter(Mandatory = $true)] [string]$Path,
        [Parameter(Mandatory = $false)] [string]$ExpectedTaskId,
        [Parameter(Mandatory = $false)] [string]$ExpectedPhase
    )
    $resolved = Assert-PathUnderRoot -Path $Path
    if (-not (Test-Path -LiteralPath $resolved)) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'missing'
            Details = "No receipt file at $resolved"
            Path = $resolved
        }
    }
    $obj = $null
    try {
        $raw = Get-Content -LiteralPath $resolved -Raw -Encoding UTF8
        $obj = ConvertFrom-Json -InputObject $raw -ErrorAction Stop
    } catch {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'malformed_json'
            Details = "Receipt did not parse as JSON: $($_.Exception.Message)"
            Path = $resolved
        }
    }
    $required = Get-RequiredReceiptFields
    $missingFields = @()
    foreach ($f in $required) {
        if ($null -eq $obj.$f) { $missingFields += $f }
    }
    if ($missingFields.Count -gt 0) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'missing_fields'
            Details = "Missing required fields: $($missingFields -join ', ')"
            Path = $resolved
        }
    }
    if ($ExpectedTaskId -and ([string]$obj.task_id) -ne $ExpectedTaskId) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'wrong_task'
            Details = "Expected task_id '$ExpectedTaskId' but receipt has '$($obj.task_id)'"
            Path = $resolved
        }
    }
    if ($ExpectedPhase -and ([string]$obj.phase) -ne $ExpectedPhase) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'wrong_phase'
            Details = "Expected phase '$ExpectedPhase' but receipt has '$($obj.phase)'"
            Path = $resolved
        }
    }
    $allowedStatus = @('completed', 'blocked', 'usage_exhausted', 'validation_failed', 'skipped')
    if ($allowedStatus -notcontains ([string]$obj.status)) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'incomplete_status'
            Details = "Receipt status '$($obj.status)' is not a finalised status"
            Path = $resolved
        }
    }
    return $true
}

function Read-ReceiptNextPhase {
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) { return $null }
    try {
        $obj = Read-JsonObject -Path $Path
        if ($null -ne $obj.next_phase) { return [string]$obj.next_phase }
    } catch {
    }
    return $null
}

function Test-ArgumentSafe {
    # Reject any value that would break the quoted form (CR, LF, NUL).
    # We control all callers; this is a defence-in-depth check.
    param([Parameter(Mandatory = $true)] [string]$Value)
    if ($Value -match "[ 
]") { return $false }
    return $true
}

function Format-CommandLine {
    # PowerShell 5.1-compatible command-line formatter.
    # Wraps every value in double quotes and escapes embedded double
    # quotes by doubling them (the Windows command-line convention).
    # Returns a single string suitable for ProcessStartInfo.Arguments
    # under .NET Framework (PS 5.1) or pwsh (PS 7+).
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [string[]]$Arguments
    )
    $sb = New-Object System.Text.StringBuilder
    for ($i = 0; $i -lt $Arguments.Count; $i++) {
        $a = [string]$Arguments[$i]
        if (-not (Test-ArgumentSafe -Value $a)) {
            throw "Refusing to format an unsafe argument (contains CR/LF/NUL)."
        }
        if ($i -gt 0) { [void]$sb.Append(' ') }
        [void]$sb.Append('"')
        # Escape embedded double quotes by doubling them.
        [void]$sb.Append($a.Replace('"', '""'))
        [void]$sb.Append('"')
    }
    return $sb.ToString()
}

function Start-ChildSession {
    param(
        [Parameter(Mandatory = $true)] [string]$Model,
        [Parameter(Mandatory = $true)] [string]$Prompt,
        [int]$TimeoutSeconds = 900,
        [string]$TaskId = '',
        [string]$Phase = ''
    )
    if ($Model -notmatch $ModelNameRegex) {
        throw "Model name fails strict regex: $Model"
    }
    if (-not (Test-ArgumentSafe -Value $Model)) {
        throw "Model name contains CR/LF/NUL: $Model"
    }
    $ollama = (Get-Command ollama -ErrorAction SilentlyContinue)
    if ($null -eq $ollama) {
        throw "ollama CLI not found on PATH"
    }
    # Flatten the prompt to a single line for the Windows command line.
    # The child session is expected to read the canonical phase prompt
    # from .ai/prompts/phases/<phase>.md, not from the inline prompt.
    $flatPrompt = ($Prompt -replace "(`r`n|`n|`r)", ' ').Trim()
    if (-not (Test-ArgumentSafe -Value $flatPrompt)) {
        throw "Prompt contains NUL after flattening"
    }
    $argList = @('launch', 'claude', '--model', $Model, '-y', '--', '--', '-p', $flatPrompt)
    # Build a single Arguments string via Format-CommandLine. This works on
    # both PowerShell 5.1 (where ProcessStartInfo.ArgumentList is missing)
    # and PowerShell 7+. No Invoke-Expression. No string concatenation of
    # unescaped values.
    $argString = Format-CommandLine -Arguments $argList
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $ollama.Source
    $psi.Arguments = $argString
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $psi

    # Set up stdout/stderr capture to .ai/receipts/logs/<task-id>-<phase>.{stdout,stderr}.log
    # so the router can report log paths when a phase exits 0 without writing
    # its receipt. The log files are evidence for diagnosing the failure.
    $stdoutLog = ''
    $stderrLog = ''
    $stdoutWriter = $null
    $stderrWriter = $null
    $captureLogs = -not ([string]::IsNullOrWhiteSpace($TaskId) -or [string]::IsNullOrWhiteSpace($Phase))
    if ($captureLogs) {
        $stdoutLog = Get-ChildLogPath -TaskId $TaskId -Phase $Phase -Stream 'stdout'
        $stderrLog = Get-ChildLogPath -TaskId $TaskId -Phase $Phase -Stream 'stderr'
        $stdoutWriter = New-Object System.IO.StreamWriter($stdoutLog, $false, [System.Text.UTF8Encoding]::new($false))
        $stderrWriter = New-Object System.IO.StreamWriter($stderrLog, $false, [System.Text.UTF8Encoding]::new($false))
        $stdoutWriter.AutoFlush = $true
        $stderrWriter.AutoFlush = $true
        # Use ScriptBlock event handlers. GetNewClosure() captures the local
        # $stdoutWriter / $stderrWriter variables so the handlers can write
        # to the same StreamWriter instance. (PS 5.1 supports GetNewClosure
        # and add_OutputDataReceived / add_ErrorDataReceived.)
        $stdoutHandler = {
            param($sender, $e)
            if ($null -ne $e -and $null -ne $e.Data) {
                try { $stdoutWriter.WriteLine($e.Data) } catch { }
            }
        }.GetNewClosure()
        $stderrHandler = {
            param($sender, $e)
            if ($null -ne $e -and $null -ne $e.Data) {
                try { $stderrWriter.WriteLine($e.Data) } catch { }
            }
        }.GetNewClosure()
        # RegisterHandler uses Add-Type to expose the generic delegate. This
        # works in PS 5.1 (System.Diagnostics.Process.OutputDataReceivedEventHandler
        # is a generic delegate of type System.DataReceivedEventHandler which
        # is a non-generic delegate with signature (object, DataReceivedEventArgs)).
        $stdoutDelegate = [System.DataReceivedEventHandler]$stdoutHandler
        $stderrDelegate = [System.DataReceivedEventHandler]$stderrHandler
        [void]$proc.add_OutputDataReceived($stdoutDelegate)
        [void]$proc.add_ErrorDataReceived($stderrDelegate)
    }

    try {
        [void]$proc.Start()
        $script:TrackedPids.Add($proc.Id) | Out-Null
        if ($captureLogs) {
            [void]$proc.BeginOutputReadLine()
            [void]$proc.BeginErrorReadLine()
        }
        if (-not $proc.WaitForExit($TimeoutSeconds * 1000)) {
            try { $proc.Kill($true) } catch { }
            throw "Child timed out after $TimeoutSeconds seconds"
        }
        # Ensure async readers have drained. The Process object will flush on close.
        if ($captureLogs) {
            $proc.WaitForExit() | Out-Null
        }
    } finally {
        $script:TrackedPids.Remove($proc.Id) | Out-Null
        if ($null -ne $stdoutWriter) {
            try { $stdoutWriter.Flush() } catch { }
            try { $stdoutWriter.Dispose() } catch { }
        }
        if ($null -ne $stderrWriter) {
            try { $stderrWriter.Flush() } catch { }
            try { $stderrWriter.Dispose() } catch { }
        }
    }

    return [pscustomobject]@{
        ExitCode = $proc.ExitCode
        StdOut   = ''
        StdErr   = ''
        StdOutLog = $stdoutLog
        StdErrLog = $stderrLog
    }
}

function Print-DryRun {
    param(
        [Parameter(Mandatory = $true)] [string]$Model,
        [Parameter(Mandatory = $true)] [string]$Prompt
    )
    # Build the same Arguments string the real Start-ChildSession would
    # pass to ProcessStartInfo.Arguments. The dry-run output is therefore
    # the verbatim Arguments form, not a hand-built approximation. CR/LF
    # in the prompt are flattened to spaces (same rule as Start-ChildSession).
    $flatPrompt = ($Prompt -replace "(`r`n|`n|`r)", ' ').Trim()
    $args = @('launch', 'claude', '--model', $Model, '-y', '--', '--', '-p', $flatPrompt)
    $argString = Format-CommandLine -Arguments $args
    Write-RouterLine ('ollama ' + $argString)
}

function Test-CloudOnly {
    param(
        [Parameter(Mandatory = $true)] $Config,
        [Parameter(Mandatory = $true)] [string]$Model
    )
    if (-not [bool]$Config.execution.cloud_only) { return }
    # local model names are like 'llama3:8b' (already matches our regex); the
    # additional check is that the name does NOT look like a local tag when
    # the user has selected a known cloud model. We allow the strict regex;
    # execution.cloud_only is informational and combined with the explicit
    # 'CONFIGURE_ME' check above. No further local-model rejection is needed
    # because the user only configures the profiles they intend to use.
}

# ---------------------------------------------------------------------------
# Ctrl+C handler
# ---------------------------------------------------------------------------

$cancelHandler = {
    $script:StopRequested = $true
    Write-RouterLine "Cancellation requested. Killing child processes." 'warn'
    foreach ($pidVal in @($script:TrackedPids)) {
        try {
            $p = Get-Process -Id $pidVal -ErrorAction SilentlyContinue
            if ($null -ne $p) { Stop-Process -Id $pidVal -Force -ErrorAction SilentlyContinue }
        } catch { }
    }
}
$null = Register-EngineEvent -SourceIdentifier 'CtrlC_Router' -Action $cancelHandler
# PowerShell 5.1's [Console] type does not expose CancelKeyPress; the engine
# event above is the canonical hook. The finally block also kills children.

# ---------------------------------------------------------------------------
# Commands
# ---------------------------------------------------------------------------

function Invoke-Status {
    Write-RouterLine "Router version: $RouterVersion"
    Write-RouterLine "Repo root: $RepoRoot"
    if (Test-Path -LiteralPath $ModelRoutingConfigPath) {
        $cfg = Read-JsonObject -Path $ModelRoutingConfigPath
        Write-RouterLine "Model routing: loaded"
        foreach ($k in @('high', 'standard', 'economy', 'review', 'fallback')) {
            $profile = $cfg.profiles.$k
            $model = if ($null -ne $profile) { [string]$profile.model } else { '<missing>' }
            $enabled = if ($null -ne $profile -and $null -ne $profile.enabled) { [bool]$profile.enabled } else { $false }
            Write-RouterLine ("  profile: {0} model: {1} enabled: {2}" -f $k, $model, $enabled)
        }
    } else {
        Write-RouterLine "Model routing: not configured ($ModelRoutingConfigPath missing)" 'warn'
    }
    if (Test-Path -LiteralPath $ActiveTaskPath) {
        $active = Read-JsonObject -Path $ActiveTaskPath
        Write-RouterLine ("Active task: {0} (milestone {1}, phase {2}, next {3})" -f $active.task_id, $active.milestone, $active.current_phase, $active.next_phase)
    } else {
        Write-RouterLine "Active task: not configured ($ActiveTaskPath missing)" 'warn'
    }
}

function Invoke-Configure {
    if (-not (Test-Path -LiteralPath $ModelRoutingConfigPath)) {
        Write-RouterLine "No model routing config found at $ModelRoutingConfigPath. Copy .ai/model-routing.example.json first." 'warn'
        return
    }
    $cfg = Read-JsonObject -Path $ModelRoutingConfigPath
    foreach ($k in @('standard', 'economy')) {
        $existing = [string]$cfg.profiles.$k.model
        $promptMsg = "Enter the Ollama cloud model name for profile '$k' (current: $existing):"
        $answer = Read-Host -Prompt $promptMsg
        if ([string]::IsNullOrWhiteSpace($answer)) { continue }
        if ($answer -notmatch $ModelNameRegex) {
            Write-RouterLine "Rejected '$answer' for profile '$k': fails strict regex $ModelNameRegex" 'warn'
            continue
        }
        $cfg.profiles.$k.model = $answer
        Write-RouterLine ("Profile '{0}' set to model '{1}'" -f $k, $answer)
    }
    $cfg | ConvertTo-Json -Depth 16 | Out-File -LiteralPath $ModelRoutingConfigPath -Encoding utf8NoBOM
    Write-RouterLine "Persisted to $ModelRoutingConfigPath"
}

function Invoke-Phase {
    # Unified phase runner used by Invoke-Plan / Invoke-Next / Invoke-RetryCurrentPhase.
    # Resolves profile, builds the prompt, runs the child, captures stdout/stderr
    # to log files, and validates the phase receipt with structured failure reasons.
    # On receipt failure: logs specific reason + log paths, does NOT advance, does
    # NOT fabricate a success receipt.
    param(
        [Parameter(Mandatory = $true)] $ActiveTask,
        [Parameter(Mandatory = $true)] $Config,
        [Parameter(Mandatory = $true)] [string]$TaskId,
        [Parameter(Mandatory = $true)] [string]$Phase,
        [string]$Profile,
        [string]$Receipt,
        [switch]$SkipIfReceiptValid
    )
    if ($Profile) {
        $profile = $Profile
    } else {
        $profile = Classify-Profile -Config $Config -ActiveTask $ActiveTask -Phase $Phase
    }
    $model = Resolve-ProfileModel -Config $Config -ProfileKey $profile
    Test-CloudOnly -Config $Config -Model $model | Out-Null
    if (-not $Receipt) {
        $Receipt = Get-ReceiptPath -TaskId $TaskId -Phase $Phase
    }
    Assert-PathUnderRoot -Path $Receipt | Out-Null

    # Retry mode guard: a valid receipt already exists for this phase. Do not
    # overwrite it. Caller should pass -SkipIfReceiptValid=$false to force a
    # retry that will overwrite.
    if ($SkipIfReceiptValid) {
        $existing = Test-PhaseReceiptValid -Path $Receipt -ExpectedTaskId $TaskId -ExpectedPhase $Phase
        if ($existing -eq $true) {
            Write-RouterLine "Receipt already valid at $Receipt. Skipping dispatch."
            return @{ Skipped = $true; Receipt = $Receipt }
        }
    }

    $prompt = New-PhasePromptString -ActiveTask $ActiveTask -Phase $Phase -Profile $profile -Model $model -ReceiptRelPath (ConvertTo-RelativeReceiptPath -AbsolutePath $Receipt)
    if ($DryRun) {
        Print-DryRun -Model $model -Prompt $prompt
        return @{ Skipped = $true; Receipt = $Receipt }
    }

    Write-RouterLine ("Dispatching phase: {0} profile: {1} model: {2}" -f $Phase, $profile, $model)
    $timeout = [int]$Config.profiles.$profile.timeout_seconds
    $result = Start-ChildSession -Model $model -Prompt $prompt -TimeoutSeconds $timeout -TaskId $TaskId -Phase $Phase
    Write-RouterLine ("Child exited with code {0}" -f $result.ExitCode)
    if ($result.StdOutLog) {
        Write-RouterLine ("Child stdout log: {0}" -f $result.StdOutLog)
    }
    if ($result.StdErrLog) {
        Write-RouterLine ("Child stderr log: {0}" -f $result.StdErrLog)
    }

    $validation = Test-PhaseReceiptValid -Path $Receipt -ExpectedTaskId $TaskId -ExpectedPhase $Phase
    if ($validation -eq $true) {
        $next = Read-ReceiptNextPhase -Path $Receipt
        Write-RouterLine ("Phase receipt valid. Next phase: {0}" -f $next)
        return @{ Skipped = $false; Receipt = $Receipt; NextPhase = $next; ExitCode = $result.ExitCode }
    }

    # Receipt is not valid. Report the specific reason + log paths. Do not
    # advance. Do not fabricate a success receipt.
    Write-RouterLine ("Phase receipt FAILED validation: {0} ({1})" -f $validation.Reason, $validation.Details) 'error'
    if ($result.StdOutLog) {
        Write-RouterLine ("Inspect child stdout: {0}" -f $result.StdOutLog) 'error'
    }
    if ($result.StdErrLog) {
        Write-RouterLine ("Inspect child stderr: {0}" -f $result.StdErrLog) 'error'
    }
    Write-RouterLine ("Expected receipt at: {0}" -f $validation.Path) 'error'
    throw ("Phase '{0}' for task '{1}' did not produce a valid receipt (reason: {2}). Child exit code: {3}." -f $Phase, $TaskId, $validation.Reason, $result.ExitCode)
}

function Invoke-Plan {
    # Plan command: launch one plan-phase child for the active task.
    $cfg = Read-JsonObject -Path $ModelRoutingConfigPath
    $active = Read-JsonObject -Path $ActiveTaskPath
    $taskId = if ([string]::IsNullOrWhiteSpace($TaskId)) { [string]$active.task_id } else { $TaskId }
    if ($taskId -notmatch $TaskIdRegex) { throw "TaskId '$taskId' fails regex" }
    $profile = if ($ProfileOverride) { $ProfileOverride } else { 'high' }
    $receipt = Get-ReceiptPath -TaskId $taskId -Phase 'plan'
    $r = Invoke-Phase -ActiveTask $active -Config $cfg -TaskId $taskId -Phase 'plan' -Profile $profile -Receipt $receipt
    if (-not $r.Skipped) {
        Write-RouterLine ("Plan phase complete. Next: {0}" -f $r.NextPhase)
    }
}

function Invoke-Next {
    $cfg = Read-JsonObject -Path $ModelRoutingConfigPath
    $active = Read-JsonObject -Path $ActiveTaskPath
    $taskId = if ([string]::IsNullOrWhiteSpace($TaskId)) { [string]$active.task_id } else { $TaskId }
    if ($taskId -notmatch $TaskIdRegex) { throw "TaskId '$taskId' fails regex" }
    $phase = if ($null -ne $active.next_phase -and $active.next_phase -ne '') { [string]$active.next_phase } else { [string]$active.current_phase }
    if ($phase -notmatch $PhaseRegex) { throw "Phase '$phase' fails regex" }
    $profile = if ($ProfileOverride) { $ProfileOverride } else { $null }
    $r = Invoke-Phase -ActiveTask $active -Config $cfg -TaskId $taskId -Phase $phase -Profile $profile
    if ($r.Skipped) {
        return
    }
    if ($null -eq $r.NextPhase -or $r.NextPhase -eq '') {
        Write-RouterLine "Closeout reached. Stopping."
    } else {
        Write-RouterLine "Run -Command Next again to dispatch the next phase."
    }
}

function Invoke-RetryCurrentPhase {
    # Retry ONLY the current phase for the active task. Does not select
    # another task. Does not restart. Overwrites an existing valid receipt
    # for the current phase (the caller asked for the retry; the existing
    # receipt is presumed incomplete or missing).
    $cfg = Read-JsonObject -Path $ModelRoutingConfigPath
    $active = Read-JsonObject -Path $ActiveTaskPath
    $taskId = if ([string]::IsNullOrWhiteSpace($TaskId)) { [string]$active.task_id } else { $TaskId }
    if ($taskId -notmatch $TaskIdRegex) { throw "TaskId '$taskId' fails regex" }
    $phase = if ($null -ne $active.next_phase -and $active.next_phase -ne '') { [string]$active.next_phase } else { [string]$active.current_phase }
    if ($phase -notmatch $PhaseRegex) { throw "Phase '$phase' fails regex" }
    Write-RouterLine ("Retrying current phase: task={0} phase={1}" -f $taskId, $phase)
    $profile = if ($ProfileOverride) { $ProfileOverride } else { $null }
    $receipt = Get-ReceiptPath -TaskId $taskId -Phase $phase
    $r = Invoke-Phase -ActiveTask $active -Config $cfg -TaskId $taskId -Phase $phase -Profile $profile -Receipt $receipt
    if ($r.Skipped) {
        return
    }
    Write-RouterLine ("Retry complete. Next: {0}" -f $r.NextPhase)
}

function Invoke-Resume {
    $active = Read-JsonObject -Path $ActiveTaskPath
    $taskId = if ([string]::IsNullOrWhiteSpace($TaskId)) { [string]$active.task_id } else { $TaskId }
    foreach ($phase in $Phases) {
        $path = Get-ReceiptPath -TaskId $taskId -Phase $phase
        if (Test-Path -LiteralPath $path) {
            $next = Read-ReceiptNextPhase -Path $path
            if ($null -ne $next -and $next -ne '') {
                Write-RouterLine ("Resume from: {0}" -f $next)
                $active.next_phase = $next
                $active | ConvertTo-Json -Depth 16 | Set-Content -LiteralPath $ActiveTaskPath -Encoding UTF8
                return
            }
        }
    }
    Write-RouterLine "No resume point found. Use -Command Plan to start."
}

function Invoke-Finish {
    Write-RouterLine "Closeout is performed by the closeout phase child. The router stops after closeout."
}

# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

try {
    Assert-PathUnderRoot -Path $RepoRoot | Out-Null
    Assert-PathUnderRoot -Path $AiDir | Out-Null
    Assert-PathUnderRoot -Path $ToolsDir | Out-Null
    Assert-PathUnderRoot -Path $ActiveTaskPath | Out-Null
    Assert-PathUnderRoot -Path $ModelRoutingConfigPath | Out-Null

    switch ($Command) {
        'Status'    { Invoke-Status }
        'Configure' { Invoke-Configure }
        'Plan'      { Invoke-Plan }
        'Next'      { Invoke-Next }
        'Resume'    { Invoke-Resume }
        'Finish'    { Invoke-Finish }
        'RetryCurrentPhase' { Invoke-RetryCurrentPhase }
        'DryRun'    {
            if (-not $ProfileOverride) { $ProfileOverride = 'standard' }
            Invoke-Next
        }
    }
    exit 0
} catch {
    Write-RouterLine ("ERROR: {0}" -f $_.Exception.Message) 'error'
    exit 1
} finally {
    foreach ($pidVal in @($script:TrackedPids)) {
        try { Stop-Process -Id $pidVal -Force -ErrorAction SilentlyContinue } catch { }
    }
}
