<#
.SYNOPSIS
    Write a phase receipt atomically. Invoked by child sessions.

.DESCRIPTION
    The AI session router's child sessions must write a
    phase receipt at .ai/receipts/phases/<task-id>-<phase>.json
    before exiting. This helper accepts the receipt JSON in
    one of three ways: as a string on -Json, from a file on
    -JsonFile, or piped via the pipeline into the -Json
    parameter. The writer validates the receipt against
    .ai/templates/phase-receipt.schema.json, creates the
    parent directory if missing, writes the file as UTF-8
    (no BOM), and exits non-zero on any failure.

    The router invokes this helper on the child side. The child
    builds the receipt from the canonical schema; this helper
    does not invent fields.

.PARAMETER ReceiptPath
    Repository-relative or absolute path to the receipt file
    (e.g. .ai/receipts/phases/T-030-reconcile.json).

.PARAMETER SchemaPath
    Path to the phase-receipt schema. Defaults to
    .ai/templates/phase-receipt.schema.json.

.PARAMETER ExpectedTaskId
    Optional. The task_id the receipt must declare.

.PARAMETER ExpectedPhase
    Optional. The phase the receipt must declare.

.PARAMETER Json
    The receipt JSON text. Accepts pipeline input by value.

.PARAMETER JsonFile
    Path to a file containing the receipt JSON. Used when the
    caller prefers not to embed the JSON on the command line.

.PARAMETER Strict
    When set, exit non-zero if the receipt is missing any
    schema-required field. Default is true.

.EXAMPLE
    $receipt | powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Write-PhaseReceipt.ps1 -ReceiptPath .ai/receipts/phases/T-030-reconcile.json -ExpectedTaskId T-030 -ExpectedPhase reconcile

.EXAMPLE
    powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Write-PhaseReceipt.ps1 -ReceiptPath .ai/receipts/phases/T-030-reconcile.json -Json $receiptJson -ExpectedTaskId T-030 -ExpectedPhase reconcile
#>

# The writer is wrapped in an advanced function so it can
# accept pipeline input. The script's own param() block
# forwards all parameters to the function. The body invokes
# the function once with the script's bound parameters. This
# pattern lets a piped string reach the -Json parameter
# without the script-level InputObjectNotBound error that
# occurs when a script with no pipeline-accepting parameter
# receives piped data.
param(
    [Parameter(Mandatory = $true)]
    [string]$ReceiptPath,

    [string]$SchemaPath = '.ai/templates/phase-receipt.schema.json',

    [string]$ExpectedTaskId,

    [string]$ExpectedPhase,

    [Parameter(ValueFromPipeline = $true)]
    [AllowEmptyString()]
    [string]$Json,

    [string]$JsonFile,

    [switch]$Strict
)

function Write-PhaseReceiptInternal {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$ReceiptPath,

        [string]$SchemaPath = '.ai/templates/phase-receipt.schema.json',

        [string]$ExpectedTaskId,

        [string]$ExpectedPhase,

        [Parameter(ValueFromPipeline = $true)]
        [AllowEmptyString()]
        [string]$Json,

        [string]$JsonFile,

        [switch]$Strict
    )

    Set-StrictMode -Version Latest
    $ErrorActionPreference = 'Stop'

    # Resolve ReceiptPath to an absolute path under the repository root.
    $RepoRoot = (Resolve-Path -Path (Join-Path $PSScriptRoot '..')).Path
    if ([System.IO.Path]::IsPathRooted($ReceiptPath)) {
        $resolved = [System.IO.Path]::GetFullPath($ReceiptPath)
    } else {
        $resolved = [System.IO.Path]::GetFullPath((Join-Path $RepoRoot $ReceiptPath))
    }
    $rootFull = [System.IO.Path]::GetFullPath($RepoRoot).TrimEnd([char][System.IO.Path]::DirectorySeparatorChar)
    $rootWithSep = $rootFull + [char][System.IO.Path]::DirectorySeparatorChar
    $ok = $resolved.Equals($rootFull, [System.StringComparison]::OrdinalIgnoreCase) -or
          $resolved.StartsWith($rootWithSep, [System.StringComparison]::OrdinalIgnoreCase)
    if (-not $ok) {
        [Console]::Error.WriteLine("ERROR: receipt path is outside the repository root: $ReceiptPath")
        exit 1
    }

    # Resolve the receipt JSON source. Priority: -Json > -JsonFile > stdin.
    $raw = ''
    if (-not [string]::IsNullOrWhiteSpace($Json)) {
        $raw = $Json
    } elseif (-not [string]::IsNullOrWhiteSpace($JsonFile)) {
        $jsonFileResolved = if (-not [System.IO.Path]::IsPathRooted($JsonFile)) {
            Join-Path $RepoRoot $JsonFile
        } else {
            $JsonFile
        }
        if (-not (Test-Path -LiteralPath $jsonFileResolved)) {
            [Console]::Error.WriteLine("ERROR: json file not found: $jsonFileResolved")
            exit 1
        }
        $raw = Get-Content -LiteralPath $jsonFileResolved -Raw -Encoding UTF8
    } else {
        # Read from the OS-level standard input stream (legacy form).
        $raw = ''
        try {
            $stdin = [Console]::OpenStandardInput()
            $reader = New-Object System.IO.StreamReader($stdin, [System.Text.UTF8Encoding]::new($false))
            $raw = $reader.ReadToEnd()
            $reader.Dispose()
            $stdin.Dispose()
        } catch {
            [Console]::Error.WriteLine("ERROR: could not read stdin: $($_.Exception.Message)")
            exit 1
        }
    }
    if ([string]::IsNullOrWhiteSpace($raw)) {
        [Console]::Error.WriteLine("ERROR: no receipt JSON supplied (use -Json, -JsonFile, or pipe on stdin)")
        exit 1
    }

    try {
        $receipt = $raw | ConvertFrom-Json -ErrorAction Stop
    } catch {
        [Console]::Error.WriteLine("ERROR: receipt is not valid JSON: $($_.Exception.Message)")
        exit 1
    }

    # Lightweight structural validation against the schema's required fields.
    $schemaPathResolved = if (-not [System.IO.Path]::IsPathRooted($SchemaPath)) {
        Join-Path $RepoRoot $SchemaPath
    } else {
        $SchemaPath
    }
    if (-not (Test-Path -LiteralPath $schemaPathResolved)) {
        [Console]::Error.WriteLine("ERROR: schema not found at $schemaPathResolved")
        exit 1
    }
    $schema = Get-Content -LiteralPath $schemaPathResolved -Raw | ConvertFrom-Json
    $required = @($schema.required)

    # Use a hashtable membership test rather than `$receipt.$field`
    # so the strict-mode property-not-found exception does not fire
    # on missing fields. ConvertTo-Json / ConvertFrom-Json produces
    # PSCustomObject whose missing properties should be detected, not
    # thrown on.
    $missing = @()
    foreach ($field in $required) {
        $hasField = $false
        $value = $null
        if ($receipt.PSObject.Properties.Name -contains $field) {
            $hasField = $true
            $value = $receipt.$field
        }
        if (-not $hasField -or $null -eq $value) { $missing += $field }
    }
    if ($missing.Count -gt 0) {
        [Console]::Error.WriteLine("ERROR: receipt is missing required fields: $($missing -join ', ')")
        exit 1
    }

    # Phase-receipt specific: status must be one of the schema enum.
    $allowedStatus = @('completed', 'blocked', 'usage_exhausted', 'validation_failed', 'skipped')
    if ($allowedStatus -notcontains [string]$receipt.status) {
        [Console]::Error.WriteLine("ERROR: receipt.status '$($receipt.status)' is not in the allowed set: $($allowedStatus -join ', ')")
        exit 1
    }

    # Optional cross-checks against expected task/phase.
    if ($ExpectedTaskId -and ([string]$receipt.task_id) -ne $ExpectedTaskId) {
        [Console]::Error.WriteLine("ERROR: receipt.task_id '$($receipt.task_id)' does not match expected '$ExpectedTaskId'")
        exit 1
    }
    if ($ExpectedPhase -and ([string]$receipt.phase) -ne $ExpectedPhase) {
        [Console]::Error.WriteLine("ERROR: receipt.phase '$($receipt.phase)' does not match expected '$ExpectedPhase'")
        exit 1
    }
    if ($receipt.task_id -and ([string]$receipt.task_id) -notmatch '^T-[0-9]+$') {
        [Console]::Error.WriteLine("ERROR: receipt.task_id '$($receipt.task_id)' fails the strict task ID regex")
        exit 1
    }
    $allowedPhases = @('reconcile', 'plan', 'implement', 'validate', 'document', 'review', 'closeout')
    if ($allowedPhases -notcontains ([string]$receipt.phase)) {
        [Console]::Error.WriteLine("ERROR: receipt.phase '$($receipt.phase)' is not in the allowed set: $($allowedPhases -join ', ')")
        exit 1
    }

    # Create parent directory if missing.
    # Note: Split-Path -LiteralPath -Parent is an ambiguous parameter set
    # (LiteralPath is not in the ParentSet). Use -Path for both calls.
    $parent = Split-Path -Path $resolved -Parent
    if (-not (Test-Path -Path $parent)) {
        [void](New-Item -ItemType Directory -Path $parent -Force)
    }

    # Write the file as UTF-8 (no BOM) atomically: write to a tmp file, then move.
    $tmp = $resolved + '.tmp'
    $json = $receipt | ConvertTo-Json -Depth 16
    [System.IO.File]::WriteAllText($tmp, $json, [System.Text.UTF8Encoding]::new($false))
    Move-Item -LiteralPath $tmp -Destination $resolved -Force

    # Read back and verify it parses.
    try {
        $verify = Get-Content -LiteralPath $resolved -Raw | ConvertFrom-Json
    } catch {
        [Console]::Error.WriteLine("ERROR: receipt did not round-trip parse after write: $($_.Exception.Message)")
        exit 1
    }
    if ($null -eq $verify.status) {
        [Console]::Error.WriteLine("ERROR: round-tripped receipt is missing status")
        exit 1
    }

    [Console]::Out.WriteLine($resolved)
}

# Entry point: forward the script's own parameters to the
# function so callers can pipe to the script or pass -Json
# the same way they would call a function.
Write-PhaseReceiptInternal @PSBoundParameters
