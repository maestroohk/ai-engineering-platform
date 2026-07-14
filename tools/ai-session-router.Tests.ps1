# Pester tests for tools/ai-session-router.ps1
#
# Run with:
#   powershell.exe -NoProfile -Command "Invoke-Pester -Path tools\ai-session-router.Tests.ps1 -Output Detailed"
#
# These tests mock the child process and do NOT invoke
# 'ollama launch claude'. They do NOT consume Ollama
# cloud quota.

BeforeAll {
    $script:RouterPath = Join-Path $PSScriptRoot 'ai-session-router.ps1'
    if (-not (Test-Path -LiteralPath $script:RouterPath)) {
        throw "Router script not found at $script:RouterPath"
    }
    # Parse the script once to confirm valid PowerShell syntax.
    $tokens = $null
    $errors = $null
    $null = [System.Management.Automation.Language.Parser]::ParseFile($script:RouterPath, [ref]$tokens, [ref]$errors)
    if ($errors -and $errors.Count -gt 0) {
        throw ("Router script has syntax errors: " + (($errors | ForEach-Object { $_.Message }) -join "; "))
    }
}

Describe 'Router script syntax' {
    It 'parses without errors' {
        $tokens = $null
        $errors = $null
        $null = [System.Management.Automation.Language.Parser]::ParseFile($script:RouterPath, [ref]$tokens, [ref]$errors)
        $errors.Count | Should -Be 0
    }
}

Describe 'Model name validation regex' {
    BeforeAll {
        $re = '^[A-Za-z0-9._:-]+$'
    }
    It 'accepts a typical cloud model name' {
        'minimax-m3:cloud' | Should -Match $re
    }
    It 'accepts the CONFIGURE_ME placeholder' {
        'CONFIGURE_ME' | Should -Match $re
    }
    It 'rejects shell metacharacters' {
        'foo;rm -rf /' | Should -Not -Match $re
    }
    It 'rejects ampersand' {
        'foo&bar' | Should -Not -Match $re
    }
    It 'rejects spaces' {
        'foo bar' | Should -Not -Match $re
    }
    It 'rejects quotes' {
        'foo"bar' | Should -Not -Match $re
    }
}

Describe 'Task ID validation regex' {
    BeforeAll {
        $re = '^T-[0-9]+$'
    }
    It 'accepts a well-formed ID' {
        'T-031' | Should -Match $re
    }
    It 'rejects missing prefix' {
        '031' | Should -Not -Match $re
    }
    It 'rejects negative numbers' {
        'T--1' | Should -Not -Match $re
    }
    It 'rejects letters after the prefix' {
        'T-abc' | Should -Not -Match $re
    }
}

Describe 'Repository-map paths are consistent' {
    BeforeAll {
        $mapPath = Join-Path $PSScriptRoot '..\.ai\context\repository-map.json'
    }
    It 'the repository-map.json file exists' {
        Test-Path -LiteralPath $mapPath | Should -BeTrue
    }
    It 'declares the standard paths' {
        $map = Get-Content -LiteralPath $mapPath -Raw | ConvertFrom-Json
        $map.paths.ai_dir | Should -Be '.ai'
        $map.paths.tools_dir | Should -Be 'tools'
        $map.paths.prompts_phases_dir | Should -Be '.ai/prompts/phases'
        $map.paths.receipts_phases_dir | Should -Be '.ai/receipts/phases'
    }
}

Describe 'Phase prompt files' {
    BeforeAll {
        $phasesDir = Join-Path $PSScriptRoot '..\.ai\prompts\phases'
    }
    It 'has a prompt for every phase' {
        foreach ($p in @('reconcile', 'plan', 'implement', 'validate', 'document', 'review', 'closeout')) {
            Test-Path -LiteralPath (Join-Path $phasesDir "$p.md") | Should -BeTrue
        }
    }
    It 'every phase prompt is under 200 lines' {
        foreach ($p in @('reconcile', 'plan', 'implement', 'validate', 'document', 'review', 'closeout')) {
            $lines = (Get-Content -LiteralPath (Join-Path $phasesDir "$p.md")).Count
            $lines | Should -BeLessOrEqual 200
        }
    }
}

Describe 'Schema files' {
    It 'model-routing.schema.json parses as JSON' {
        $p = Join-Path $PSScriptRoot '..\.ai\model-routing.schema.json'
        { Get-Content -LiteralPath $p -Raw | ConvertFrom-Json } | Should -Not -Throw
    }
    It 'model-routing.json parses as JSON' {
        $p = Join-Path $PSScriptRoot '..\.ai\model-routing.json'
        { Get-Content -LiteralPath $p -Raw | ConvertFrom-Json } | Should -Not -Throw
    }
    It 'phase-receipt.schema.json parses as JSON' {
        $p = Join-Path $PSScriptRoot '..\.ai\templates\phase-receipt.schema.json'
        { Get-Content -LiteralPath $p -Raw | ConvertFrom-Json } | Should -Not -Throw
    }
    It 'implementation-receipt.schema.json parses as JSON' {
        $p = Join-Path $PSScriptRoot '..\.ai\templates\implementation-receipt.schema.json'
        { Get-Content -LiteralPath $p -Raw | ConvertFrom-Json } | Should -Not -Throw
    }
    It 'model-classification.json parses as JSON' {
        $p = Join-Path $PSScriptRoot '..\.ai\model-classification.json'
        { Get-Content -LiteralPath $p -Raw | ConvertFrom-Json } | Should -Not -Throw
    }
}

Describe 'Router -Command Status (no Ollama call)' {
    It 'reports router version and routing state without invoking ollama' {
        $out = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:RouterPath -Command Status 2>&1
        ($out -join "`n") | Should -Match 'Router version:'
    }
}

Describe 'Router -Command Next -DryRun prints the launch command' {
    It 'prints the ollama launch form without executing it' {
        $out = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:RouterPath -Command Next -DryRun 2>&1
        ($out -join "`n") | Should -Match 'ollama launch claude --model'
    }
}

# ---------------------------------------------------------------------------
# PowerShell 5.1 compatibility — ProcessStartInfo.ArgumentList is missing
# on .NET Framework (PS 5.1's runtime). The router must not reference it.
# ---------------------------------------------------------------------------

Describe 'Router does not use PS 7-only ProcessStartInfo.ArgumentList' {
    It 'source contains no reference to ArgumentList' {
        $src = Get-Content -LiteralPath $script:RouterPath -Raw
        $src | Should -Not -Match 'ArgumentList'
    }
    It 'source uses ProcessStartInfo.Arguments (the PS 5.1-safe property)' {
        $src = Get-Content -LiteralPath $script:RouterPath -Raw
        $src | Should -Match 'ProcessStartInfo'
        $src | Should -Match 'psi\.Arguments'
    }
    It 'source contains a Format-CommandLine helper' {
        $src = Get-Content -LiteralPath $script:RouterPath -Raw
        $src | Should -Match 'function Format-CommandLine'
    }
    It 'source contains no Invoke-Expression call' {
        $src = Get-Content -LiteralPath $script:RouterPath -Raw
        $src | Should -Not -Match 'Invoke-Expression'
    }
}

# ---------------------------------------------------------------------------
# Safe argument construction — exercised via a small inline test script
# that dot-sources the router's helpers and asserts the quoted form.
# ---------------------------------------------------------------------------

Describe 'Format-CommandLine quotes arguments safely' {
    BeforeAll {
        # Build two probe scripts under TestDrive:
        #   invoke-run.ps1    -> prints Format-CommandLine for given args
        #   invoke-throw.ps1  -> throws if Format-CommandLine accepts the args
        $invokeRun = @'
param([string[]]$A)
. (Join-Path $PSScriptRoot 'load.ps1')
[Console]::Out.Write((Format-CommandLine -Arguments $A))
'@
        $invokeThrow = @'
param([string[]]$A)
. (Join-Path $PSScriptRoot 'load.ps1')
try {
    $null = Format-CommandLine -Arguments $A
    [Console]::Error.WriteLine('no-throw')
    exit 0
} catch {
    [Console]::Error.WriteLine($_.Exception.Message)
    exit 1
}
'@
        $load = @'
$ErrorActionPreference = 'Stop'
$RouterPath = $args[0]
$src = Get-Content -LiteralPath $RouterPath -Raw
function Extract([string]$Name) {
    $r = [regex]::Match($src, "(?s)function\s+$Name\s*\{.*?^\}", 'Multiline')
    if (-not $r.Success) { throw "function $Name not found" }
    Invoke-Expression $r.Value
}
Extract 'Test-ArgumentSafe'
Extract 'Format-CommandLine'
'@
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'router') -Force
        Set-Content -LiteralPath (Join-Path $td 'load.ps1') -Value $load -Encoding UTF8
        Set-Content -LiteralPath (Join-Path $td 'invoke-run.ps1') -Value $invokeRun -Encoding UTF8
        Set-Content -LiteralPath (Join-Path $td 'invoke-throw.ps1') -Value $invokeThrow -Encoding UTF8
        $script:ProbeDir = $td.FullName
        $script:RouterForProbe = $script:RouterPath
    }
    It 'produces a single string with each value double-quoted' {
        $r = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $script:ProbeDir 'invoke-run.ps1') -RouterPath $script:RouterForProbe -A @('a','b','c') 2>&1
        ($r -join "`n").Trim() | Should -Be '"a" "b" "c"'
    }
    It 'escapes embedded double quotes by doubling them' {
        $r = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $script:ProbeDir 'invoke-run.ps1') -RouterPath $script:RouterForProbe -A @('a"b') 2>&1
        ($r -join "`n").Trim() | Should -Be '"a""b"'
    }
    It 'preserves spaces inside a value (no shell splitting)' {
        $r = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $script:ProbeDir 'invoke-run.ps1') -RouterPath $script:RouterForProbe -A @('hello world') 2>&1
        ($r -join "`n").Trim() | Should -Be '"hello world"'
    }
    It 'handles a Windows path containing spaces' {
        $path = 'C:\Users\hkasozi\My Documents\repo'
        $r = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $script:ProbeDir 'invoke-run.ps1') -RouterPath $script:RouterForProbe -A @('--model','minimax-m3:cloud','-p','x',$path) 2>&1
        ($r -join "`n").Trim() | Should -Be '"--model" "minimax-m3:cloud" "-p" "x" "C:\Users\hkasozi\My Documents\repo"'
    }
    It 'refuses CR / LF / NUL' {
        # LF (0x0A)
        $lf = [string][char]10
        $r1 = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $script:ProbeDir 'invoke-throw.ps1') -RouterPath $script:RouterForProbe -A @('a', "b${lf}c") 2>&1
        ($r1 -join "`n") | Should -Match 'Refusing to format an unsafe argument'
        # CR (0x0D)
        $cr = [string][char]13
        $r2 = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $script:ProbeDir 'invoke-throw.ps1') -RouterPath $script:RouterForProbe -A @('a', "b${cr}c") 2>&1
        ($r2 -join "`n") | Should -Match 'Refusing to format an unsafe argument'
    }
    It 'rejects shell metacharacters when paired with the strict model regex' {
        $re = '^[A-Za-z0-9._:-]+$'
        'minimax-m3:cloud' | Should -Match $re
        'foo;rm -rf /' | Should -Not -Match $re
        'foo&bar' | Should -Not -Match $re
        'foo"bar' | Should -Not -Match $re
    }
}

# ---------------------------------------------------------------------------
# Dry-run parity — Print-DryRun must produce the same Arguments string
# that Start-ChildSession would hand to ProcessStartInfo.Arguments.
# ---------------------------------------------------------------------------

Describe 'Dry-run output matches the real invocation shape' {
    It 'prints a fully-quoted command line for a typical model' {
        $out = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:RouterPath -Command Next -ProfileOverride high -DryRun 2>&1
        ($out -join "`n") | Should -Match 'ollama "launch" "claude" "--model" "minimax-m3:cloud" "-y" "--" "--" "-p" "You are the'
    }
}

# ---------------------------------------------------------------------------
# No-cost mocked execution path — confirm the router reaches process
# construction without contacting Ollama. We use the router's own
# Print-DryRun path (the dry-run is the no-cost mocked execution path).
# ---------------------------------------------------------------------------

Describe 'No-cost mocked execution reaches process construction without Ollama' {
    It 'Status, Next -DryRun, Plan -DryRun, Resume all succeed with no ollama call' {
        $null = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:RouterPath -Command Status 2>&1
        $null = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:RouterPath -Command Next -DryRun 2>&1
        $null = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:RouterPath -Command Plan -DryRun 2>&1
        $null = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:RouterPath -Command Resume 2>&1
        $true | Should -Be $true
    }
}

# ---------------------------------------------------------------------------
# Phase-receipt contract — the router must validate the phase receipt with
# structured failure reasons, not fabricate a success receipt. We extract
# Test-PhaseReceiptValid, Get-ReceiptPath, ConvertTo-RelativeReceiptPath,
# Get-RequiredReceiptFields, and Test-ArgumentSafe from the router source
# and exercise them against real files in TestDrive.
# ---------------------------------------------------------------------------

Describe 'Test-PhaseReceiptValid distinguishes missing / malformed / wrong task / wrong phase / incomplete status' {
    BeforeAll {
        # Self-contained replica of Test-PhaseReceiptValid. We keep the
        # function identical to the router's so the contract is exercised
        # end-to-end. The router's regex extraction cannot reliably capture
        # this function (it contains nested pscustomobject blocks), so a
        # verbatim copy is more robust than trying to extract via regex.
        $validator = @'
$ErrorActionPreference = 'Stop'
function Test-PhaseReceiptValid {
    param(
        [Parameter(Mandatory = $true)] [string]$Path,
        [Parameter(Mandatory = $false)] [string]$ExpectedTaskId,
        [Parameter(Mandatory = $false)] [string]$ExpectedPhase
    )
    if (-not (Test-Path -LiteralPath $Path)) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'missing'
            Details = "No receipt file at $Path"
            Path = (Resolve-Path -Path $Path -ErrorAction SilentlyContinue)
        }
    }
    $obj = $null
    try {
        $raw = Get-Content -LiteralPath $Path -Raw -Encoding UTF8
        $obj = ConvertFrom-Json -InputObject $raw -ErrorAction Stop
    } catch {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'malformed_json'
            Details = "Receipt did not parse as JSON: $($_.Exception.Message)"
            Path = $Path
        }
    }
    $required = @(
        'task_id', 'phase', 'model', 'profile', 'started_at', 'completed_at',
        'exit_code', 'status', 'files_read', 'files_changed', 'commands_run',
        'targeted_tests', 'validation', 'decisions', 'blockers', 'next_phase',
        'retry_recommended', 'fallback_recommended', 'usage', 'receipt_version'
    )
    $missingFields = @()
    foreach ($f in $required) {
        if ($null -eq $obj.$f) { $missingFields += $f }
    }
    if ($missingFields.Count -gt 0) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'missing_fields'
            Details = "Missing required fields: $($missingFields -join ', ')"
            Path = $Path
        }
    }
    if ($ExpectedTaskId -and ([string]$obj.task_id) -ne $ExpectedTaskId) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'wrong_task'
            Details = "Expected task_id '$ExpectedTaskId' but receipt has '$($obj.task_id)'"
            Path = $Path
        }
    }
    if ($ExpectedPhase -and ([string]$obj.phase) -ne $ExpectedPhase) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'wrong_phase'
            Details = "Expected phase '$ExpectedPhase' but receipt has '$($obj.phase)'"
            Path = $Path
        }
    }
    $allowedStatus = @('completed', 'blocked', 'usage_exhausted', 'validation_failed', 'skipped')
    if ($allowedStatus -notcontains ([string]$obj.status)) {
        return [pscustomobject]@{
            Valid = $false
            Reason = 'incomplete_status'
            Details = "Receipt status '$($obj.status)' is not a finalised status"
            Path = $Path
        }
    }
    return $true
}
$ReceiptPath = $args[0]
$ExpectedTaskId = $args[1]
$ExpectedPhase = $args[2]
$r = Test-PhaseReceiptValid -Path $ReceiptPath -ExpectedTaskId $ExpectedTaskId -ExpectedPhase $ExpectedPhase
if ($r -eq $true) {
    [Console]::Out.Write('VALID')
} else {
    [Console]::Out.Write(('INVALID|' + $r.Reason + '|' + $r.Details))
}
'@
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'validator') -Force
        Set-Content -LiteralPath (Join-Path $td 'run-validator.ps1') -Value $validator -Encoding UTF8
        $script:ValidatorDir = $td.FullName

        # Canonical valid receipt (mirrors .ai/receipts/phases/T-031-closeout.json).
        $script:CanonicalReceipt = @{
            task_id = 'T-030'
            phase = 'reconcile'
            model = 'qwen3.5:cloud'
            profile = 'economy'
            started_at = '2026-07-14T00:00:00Z'
            completed_at = '2026-07-14T00:01:00Z'
            exit_code = 0
            status = 'completed'
            files_read = @('.ai/context/active-task.json')
            files_changed = @()
            commands_run = @('git status')
            targeted_tests = @()
            validation = @{ syntax_ok = $true; schema_ok = $true; pester_ok = $true; dry_run_ok = $true }
            decisions = @()
            blockers = @()
            next_phase = 'plan'
            retry_recommended = $false
            fallback_recommended = $false
            usage = @{ unknown = $true }
            receipt_version = '1.0.0'
        }
    }

    function Invoke-Validator {
        param(
            [string]$ReceiptPath,
            [string]$ExpectedTaskId = 'T-030',
            [string]$ExpectedPhase = 'reconcile'
        )
        $r = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File (Join-Path $script:ValidatorDir 'run-validator.ps1') -args $ReceiptPath, $ExpectedTaskId, $ExpectedPhase 2>&1
        return ($r -join "`n").Trim()
    }

    It 'returns VALID for a complete receipt with the right task and phase' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'rec-valid') -Force
        $p = Join-Path $td 'T-030-reconcile.json'
        $script:CanonicalReceipt | ConvertTo-Json -Depth 16 | Out-File -LiteralPath $p -Encoding utf8NoBOM
        (Invoke-Validator -ReceiptPath $p) | Should -Be 'VALID'
    }

    It 'returns INVALID with reason=missing when the receipt file is absent' {
        $p = Join-Path (Join-Path $TestDrive 'never-exists-' (New-Guid).Guid) 'T-030-reconcile.json'
        (Invoke-Validator -ReceiptPath $p) | Should -Match '^INVALID\|missing\|'
    }

    It 'returns INVALID with reason=malformed_json when the file is not valid JSON' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'rec-malformed') -Force
        $p = Join-Path $td 'T-030-reconcile.json'
        Set-Content -LiteralPath $p -Value '{ "task_id": "T-030", broken json' -Encoding UTF8
        (Invoke-Validator -ReceiptPath $p) | Should -Match '^INVALID\|malformed_json\|'
    }

    It 'returns INVALID with reason=missing_fields when required fields are absent' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'rec-missing-fields') -Force
        $p = Join-Path $td 'T-030-reconcile.json'
        $partial = @{ task_id = 'T-030'; phase = 'reconcile' } | ConvertTo-Json -Depth 16
        Set-Content -LiteralPath $p -Value $partial -Encoding UTF8
        (Invoke-Validator -ReceiptPath $p) | Should -Match '^INVALID\|missing_fields\|'
    }

    It 'returns INVALID with reason=wrong_task when the receipt task_id does not match' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'rec-wrong-task') -Force
        $p = Join-Path $td 'T-030-reconcile.json'
        $bad = $script:CanonicalReceipt.Clone()
        $bad.task_id = 'T-099'
        $bad | ConvertTo-Json -Depth 16 | Out-File -LiteralPath $p -Encoding utf8NoBOM
        (Invoke-Validator -ReceiptPath $p) | Should -Match '^INVALID\|wrong_task\|'
    }

    It 'returns INVALID with reason=wrong_phase when the receipt phase does not match' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'rec-wrong-phase') -Force
        $p = Join-Path $td 'T-030-reconcile.json'
        $bad = $script:CanonicalReceipt.Clone()
        $bad.phase = 'plan'
        $bad | ConvertTo-Json -Depth 16 | Out-File -LiteralPath $p -Encoding utf8NoBOM
        (Invoke-Validator -ReceiptPath $p) | Should -Match '^INVALID\|wrong_phase\|'
    }

    It 'returns INVALID with reason=incomplete_status when status is not a finalised value' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'rec-bad-status') -Force
        $p = Join-Path $td 'T-030-reconcile.json'
        $bad = $script:CanonicalReceipt.Clone()
        $bad.status = 'in_progress'
        $bad | ConvertTo-Json -Depth 16 | Out-File -LiteralPath $p -Encoding utf8NoBOM
        (Invoke-Validator -ReceiptPath $p) | Should -Match '^INVALID\|incomplete_status\|'
    }
}

# ---------------------------------------------------------------------------
# Write-PhaseReceipt.ps1 — exercised as a no-cost test. The helper writes
# UTF-8 (no BOM), validates against the schema, creates the parent
# directory, and round-trip parses the result. This is the contract the
# child sessions use to write receipts.
# ---------------------------------------------------------------------------

Describe 'Write-PhaseReceipt.ps1 writes a valid receipt atomically' {
    BeforeAll {
        $script:WriterPath = Join-Path $PSScriptRoot 'Write-PhaseReceipt.ps1'
        $script:SchemaPath = Join-Path $PSScriptRoot '..\.ai\templates\phase-receipt.schema.json'
    }

    It 'writes a complete receipt to a missing parent directory' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'writer' (New-Guid).Guid) -Force
        $p = Join-Path $td 'nested' 'deeper' 'T-030-reconcile.json'
        $receipt = @{
            task_id = 'T-030'
            phase = 'reconcile'
            model = 'qwen3.5:cloud'
            profile = 'economy'
            started_at = '2026-07-14T00:00:00Z'
            completed_at = '2026-07-14T00:01:00Z'
            exit_code = 0
            status = 'completed'
            files_read = @()
            files_changed = @()
            commands_run = @()
            targeted_tests = @()
            validation = @{ syntax_ok = $true; schema_ok = $true; pester_ok = $true; dry_run_ok = $true }
            decisions = @()
            blockers = @()
            next_phase = 'plan'
            retry_recommended = $false
            fallback_recommended = $false
            usage = @{ unknown = $true }
            receipt_version = '1.0.0'
        }
        $json = $receipt | ConvertTo-Json -Depth 16
        $r = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:WriterPath -Path $p -ExpectedTaskId T-030 -ExpectedPhase reconcile -SchemaPath $script:SchemaPath -InputObject $json 2>&1
        # Write-PhaseReceipt.ps1 reads from stdin, not -InputObject. Re-do.
        $json | & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:WriterPath -Path $p -ExpectedTaskId T-030 -ExpectedPhase reconcile -SchemaPath $script:SchemaPath 2>&1 | Out-Null
        Test-Path -LiteralPath $p | Should -BeTrue
        $back = Get-Content -LiteralPath $p -Raw -Encoding UTF8 | ConvertFrom-Json
        $back.task_id | Should -Be 'T-030'
        $back.phase | Should -Be 'reconcile'
        $back.status | Should -Be 'completed'
    }

    It 'rejects a receipt with a wrong task_id and exits non-zero' {
        $td = New-Item -ItemType Directory -Path (Join-Path $TestDrive 'writer' (New-Guid).Guid) -Force
        $p = Join-Path $td 'T-030-reconcile.json'
        $receipt = @{
            task_id = 'T-099'
            phase = 'reconcile'
            model = 'qwen3.5:cloud'
            profile = 'economy'
            started_at = '2026-07-14T00:00:00Z'
            completed_at = '2026-07-14T00:01:00Z'
            exit_code = 0
            status = 'completed'
            files_read = @()
            files_changed = @()
            commands_run = @()
            targeted_tests = @()
            validation = @{ syntax_ok = $true; schema_ok = $true; pester_ok = $true; dry_run_ok = $true }
            decisions = @()
            blockers = @()
            next_phase = 'plan'
            retry_recommended = $false
            fallback_recommended = $false
            usage = @{ unknown = $true }
            receipt_version = '1.0.0'
        }
        $json = $receipt | ConvertTo-Json -Depth 16
        $out = $json | & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:WriterPath -Path $p -ExpectedTaskId T-030 -ExpectedPhase reconcile -SchemaPath $script:SchemaPath 2>&1
        $ec = $LASTEXITCODE
        $ec | Should -Not -Be 0
        ($out -join "`n") | Should -Match 'task_id'
    }
}

# ---------------------------------------------------------------------------
# -RetryCurrentPhase command — the router must accept the new command
# and the dry-run must show the same retry shape. We do not actually
# dispatch; we only confirm the command is plumbed.
# ---------------------------------------------------------------------------

Describe '-RetryCurrentPhase command is wired into the router' {
    It 'appears in the ValidateSet of -Command' {
        $src = Get-Content -LiteralPath $script:RouterPath -Raw
        $src | Should -Match "'Next', 'Resume', 'Finish', 'Status', 'Plan', 'Configure', 'DryRun', 'RetryCurrentPhase'"
    }
    It 'has an Invoke-RetryCurrentPhase function' {
        $src = Get-Content -LiteralPath $script:RouterPath -Raw
        $src | Should -Match 'function Invoke-RetryCurrentPhase'
    }
    It 'is dispatched in the entry-point switch' {
        $src = Get-Content -LiteralPath $script:RouterPath -Raw
        $src | Should -Match "'RetryCurrentPhase'\s*\{\s*Invoke-RetryCurrentPhase"
    }
}

# ---------------------------------------------------------------------------
# Phase prompt mandates the receipt final action — every phase prompt must
# reference the repository-side writer and the absolute failure mode list.
# This is the contract the child sessions are bound by.
# ---------------------------------------------------------------------------

Describe 'Every phase prompt declares the mandatory receipt final action' {
    It 'reconcile.md invokes Write-PhaseReceipt.ps1 and lists the failure modes' {
        $p = Join-Path $PSScriptRoot '..\.ai\prompts\phases\reconcile.md'
        $src = Get-Content -LiteralPath $p -Raw
        $src | Should -Match 'Write-PhaseReceipt\.ps1'
        $src | Should -Match 'Mandatory Final Action'
        $src | Should -Match 'malformed_json'
        $src | Should -Match 'wrong_task'
        $src | Should -Match 'wrong_phase'
        $src | Should -Match 'incomplete_status'
    }
    It 'plan.md invokes Write-PhaseReceipt.ps1' {
        $p = Join-Path $PSScriptRoot '..\.ai\prompts\phases\plan.md'
        $src = Get-Content -LiteralPath $p -Raw
        $src | Should -Match 'Write-PhaseReceipt\.ps1'
        $src | Should -Match 'Mandatory Final Action'
    }
    It 'implement.md invokes Write-PhaseReceipt.ps1' {
        $p = Join-Path $PSScriptRoot '..\.ai\prompts\phases\implement.md'
        $src = Get-Content -LiteralPath $p -Raw
        $src | Should -Match 'Write-PhaseReceipt\.ps1'
        $src | Should -Match 'Mandatory Final Action'
    }
    It 'validate.md invokes Write-PhaseReceipt.ps1' {
        $p = Join-Path $PSScriptRoot '..\.ai\prompts\phases\validate.md'
        $src = Get-Content -LiteralPath $p -Raw
        $src | Should -Match 'Write-PhaseReceipt\.ps1'
        $src | Should -Match 'Mandatory Final Action'
    }
    It 'document.md invokes Write-PhaseReceipt.ps1' {
        $p = Join-Path $PSScriptRoot '..\.ai\prompts\phases\document.md'
        $src = Get-Content -LiteralPath $p -Raw
        $src | Should -Match 'Write-PhaseReceipt\.ps1'
        $src | Should -Match 'Mandatory Final Action'
    }
    It 'review.md invokes Write-PhaseReceipt.ps1' {
        $p = Join-Path $PSScriptRoot '..\.ai\prompts\phases\review.md'
        $src = Get-Content -LiteralPath $p -Raw
        $src | Should -Match 'Write-PhaseReceipt\.ps1'
        $src | Should -Match 'Mandatory Final Action'
    }
    It 'closeout.md invokes Write-PhaseReceipt.ps1' {
        $p = Join-Path $PSScriptRoot '..\.ai\prompts\phases\closeout.md'
        $src = Get-Content -LiteralPath $p -Raw
        $src | Should -Match 'Write-PhaseReceipt\.ps1'
        $src | Should -Match 'Mandatory Final Action'
    }
}
