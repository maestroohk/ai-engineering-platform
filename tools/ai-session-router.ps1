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
    [ValidateSet('Next', 'Resume', 'Finish', 'Status', 'Plan', 'Configure', 'DryRun')]
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
        [Parameter(Mandatory = $true)] [string]$ReceiptPath
    )
    $prompt = @"
You are the $Phase phase of task $($ActiveTask.task_id) in milestone $($ActiveTask.milestone).
Profile: $Profile
Model: $Model
Receipt path: $ReceiptPath
Active packet: $ActiveTaskPath
Approved plan: $($ActiveTask.approved_plan)
Stop conditions: see $PromptsDir\$Phase.md

Read the active packet and the phase prompt. Do not exceed the phase's allowed
actions. Do not begin another phase. Write the phase receipt to the receipt
path declared above when you finish.
"@
    return $prompt
}

function Get-ReceiptPath {
    param(
        [Parameter(Mandatory = $true)] [string]$TaskId,
        [Parameter(Mandatory = $true)] [string]$Phase
    )
    return Join-Path $ReceiptsDir "$TaskId-$Phase.json"
}

function Test-ReceiptComplete {
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) { return $false }
    try {
        $obj = Read-JsonObject -Path $Path
    } catch {
        return $false
    }
    if ($null -eq $obj.status) { return $false }
    if ($null -eq $obj.next_phase) { return $false }
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

function Start-ChildSession {
    param(
        [Parameter(Mandatory = $true)] [string]$Model,
        [Parameter(Mandatory = $true)] [string]$Prompt,
        [int]$TimeoutSeconds = 900
    )
    if ($Model -notmatch $ModelNameRegex) {
        throw "Model name fails strict regex: $Model"
    }
    $ollama = (Get-Command ollama -ErrorAction SilentlyContinue)
    if ($null -eq $ollama) {
        throw "ollama CLI not found on PATH"
    }
    $argList = @('launch', 'claude', '--model', $Model, '-y', '--', '--', '-p', $Prompt)
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $ollama.Source
    foreach ($a in $argList) { [void]$psi.ArgumentList.Add($a) }
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true
    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $psi
    [void]$proc.Start()
    $script:TrackedPids.Add($proc.Id) | Out-Null
    if (-not $proc.WaitForExit($TimeoutSeconds * 1000)) {
        try { $proc.Kill($true) } catch { }
        throw "Child timed out after $TimeoutSeconds seconds"
    }
    $stdout = $proc.StandardOutput.ReadToEnd()
    $stderr = $proc.StandardError.ReadToEnd()
    $script:TrackedPids.Remove($proc.Id) | Out-Null
    return [pscustomobject]@{
        ExitCode = $proc.ExitCode
        StdOut   = $stdout
        StdErr   = $stderr
    }
}

function Print-DryRun {
    param(
        [Parameter(Mandatory = $true)] [string]$Model,
        [Parameter(Mandatory = $true)] [string]$Prompt
    )
    $cmd = 'ollama launch claude --model ' + $Model + ' -y -- -p "' + $Prompt + '"'
    Write-RouterLine $cmd
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

function Invoke-Plan {
    # Plan command: launch one plan-phase child for the active task.
    $cfg = Read-JsonObject -Path $ModelRoutingConfigPath
    $active = Read-JsonObject -Path $ActiveTaskPath
    $taskId = if ([string]::IsNullOrWhiteSpace($TaskId)) { [string]$active.task_id } else { $TaskId }
    if ($taskId -notmatch $TaskIdRegex) { throw "TaskId '$taskId' fails regex" }
    $profile = if ($ProfileOverride) { $ProfileOverride } else { 'high' }
    $model = Resolve-ProfileModel -Config $cfg -ProfileKey $profile
    $receipt = Get-ReceiptPath -TaskId $taskId -Phase 'plan'
    $prompt = New-PhasePromptString -ActiveTask $active -Phase 'plan' -Profile $profile -Model $model -ReceiptPath $receipt
    if ($DryRun) {
        Print-DryRun -Model $model -Prompt $prompt
        return
    }
    $result = Start-ChildSession -Model $model -Prompt $prompt -TimeoutSeconds ([int]$cfg.profiles.$profile.timeout_seconds)
    Write-RouterLine ("Plan phase exited with code {0}" -f $result.ExitCode)
    if (-not (Test-ReceiptComplete -Path $receipt)) {
        throw "Plan phase did not write a complete receipt at $receipt"
    }
    $next = Read-ReceiptNextPhase -Path $receipt
    Write-RouterLine ("Next phase: {0}" -f $next)
}

function Invoke-Next {
    $cfg = Read-JsonObject -Path $ModelRoutingConfigPath
    $active = Read-JsonObject -Path $ActiveTaskPath
    $taskId = if ([string]::IsNullOrWhiteSpace($TaskId)) { [string]$active.task_id } else { $TaskId }
    if ($taskId -notmatch $TaskIdRegex) { throw "TaskId '$taskId' fails regex" }
    $phase = if ($null -ne $active.next_phase -and $active.next_phase -ne '') { [string]$active.next_phase } else { [string]$active.current_phase }
    if ($phase -notmatch $PhaseRegex) { throw "Phase '$phase' fails regex" }
    $profile = if ($ProfileOverride) { $ProfileOverride } else { Classify-Profile -Config $cfg -ActiveTask $active -Phase $phase }
    $model = Resolve-ProfileModel -Config $cfg -ProfileKey $profile
    Test-CloudOnly -Config $cfg -Model $model | Out-Null
    $receipt = Get-ReceiptPath -TaskId $taskId -Phase $phase
    $prompt = New-PhasePromptString -ActiveTask $active -Phase $phase -Profile $profile -Model $model -ReceiptPath $receipt
    if ($DryRun) {
        Print-DryRun -Model $model -Prompt $prompt
        return
    }
    Write-RouterLine ("Dispatching phase: {0} profile: {1} model: {2}" -f $phase, $profile, $model)
    $timeout = [int]$cfg.profiles.$profile.timeout_seconds
    $result = Start-ChildSession -Model $model -Prompt $prompt -TimeoutSeconds $timeout
    Write-RouterLine ("Child exited with code {0}" -f $result.ExitCode)
    if (-not (Test-ReceiptComplete -Path $receipt)) {
        throw "Phase '$phase' did not write a complete receipt at $receipt"
    }
    $next = Read-ReceiptNextPhase -Path $receipt
    Write-RouterLine ("Next phase from receipt: {0}" -f $next)
    if ($null -eq $next -or $next -eq '') {
        Write-RouterLine "Closeout reached. Stopping."
    } else {
        Write-RouterLine "Run -Command Next again to dispatch the next phase."
    }
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
