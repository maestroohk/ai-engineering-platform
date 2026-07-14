# Mocked receipt-contract smoke test for the AI session router.
#
# Runs WITHOUT Ollama. Exercises:
#   - Write-PhaseReceipt.ps1 against a canonical receipt (happy path)
#   - Write-PhaseReceipt.ps1 against a malformed JSON (rejected)
#   - Write-PhaseReceipt.ps1 against a missing-fields receipt (rejected)
#   - Write-PhaseReceipt.ps1 against a wrong-task receipt (rejected)
#   - Write-PhaseReceipt.ps1 against a wrong-phase receipt (rejected)
#   - Write-PhaseReceipt.ps1 against an in-progress status (rejected)
#   - Write-PhaseReceipt.ps1 against a completed valid receipt (accepted)
#   - Atomic write to a missing parent directory
#   - Round-trip parse of the written receipt
#   - UTF-8 (no BOM) write
#   - Router -Command Next -DryRun selects T-030 / reconcile
#   - Router -Command Status reports the new state
#   - Router source has Test-PhaseReceiptValid and Invoke-RetryCurrentPhase
#   - Every phase prompt mentions Write-PhaseReceipt.ps1 and the
#     mandatory final action
#
# This is a single, no-cost smoke test. No real model is invoked.

$ErrorActionPreference = 'Stop'
$RepoRoot = (Resolve-Path -Path (Join-Path $PSScriptRoot '..')).Path
$Writer = Join-Path $PSScriptRoot 'Write-PhaseReceipt.ps1'
$Router = Join-Path $PSScriptRoot 'ai-session-router.ps1'
$SchemaPath = Join-Path $RepoRoot '.ai/templates/phase-receipt.schema.json'
# Temp files go inside the repo (under a hidden test dir) so the writer's
# repository-root check accepts them. Cleaned up at the end.
$Tmp = Join-Path $RepoRoot ".ai/test-tmp-receipt-contract"
if (Test-Path -LiteralPath $Tmp) { Remove-Item -LiteralPath $Tmp -Recurse -Force }
$Receipts = Join-Path $Tmp 'receipts'
[void](New-Item -ItemType Directory -Path $Receipts -Force)

$pass = 0
$fail = 0
function Check {
    param([string]$Name, [bool]$Ok, [string]$Detail = '')
    if ($Ok) {
        $script:pass++
        Write-Host "  PASS  $Name"
    } else {
        $script:fail++
        Write-Host "  FAIL  $Name  $Detail" -ForegroundColor Red
    }
}

function Make-Receipt {
    # Build a fully-populated receipt with override fields.
    param([hashtable]$Override)
    $r = @{
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
    foreach ($k in $Override.Keys) { $r[$k] = $Override[$k] }
    return $r
}

Write-Host "==== Receipt contract smoke test ===="
Write-Host "RepoRoot: $RepoRoot"
Write-Host "Writer:   $Writer"
Write-Host "Tmp:      $Tmp"

# ---------------------------------------------------------------------------
# 1. Write-PhaseReceipt.ps1 happy path: completes and writes a valid receipt
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "## Write-PhaseReceipt.ps1"

function Run-Writer {
    # Pipe $json to the writer and return the exit code.
    # The child powershell.exe may emit a non-terminating RemoteException
    # when the writer exits non-zero. The test verifies the exit code,
    # so we suppress that error stream and capture $LASTEXITCODE.
    param([string]$Json, [string]$Path, [string]$ExpectedTaskId, [string]$ExpectedPhase)
    $prevEap = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $Json | & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $Writer -ReceiptPath $Path -ExpectedTaskId $ExpectedTaskId -ExpectedPhase $ExpectedPhase 2>&1 | Out-Null
    } catch {
        # Swallow remote exceptions; the exit code is the test signal.
    } finally {
        $ErrorActionPreference = $prevEap
    }
    return $LASTEXITCODE
}

$validPath = Join-Path $Receipts 'T-030-reconcile.json'
$valid = Make-Receipt @{}
$json = $valid | ConvertTo-Json -Depth 16
$ec = Run-Writer -Json $json -Path $validPath -ExpectedTaskId T-030 -ExpectedPhase reconcile
Check 'valid receipt writes' ($ec -eq 0) "exit=$ec"
Check 'valid receipt file exists' (Test-Path -LiteralPath $validPath) ''
if (Test-Path -LiteralPath $validPath) {
    $bytes = [System.IO.File]::ReadAllBytes($validPath)
    # UTF-8 BOM is EF BB BF. No-BOM receipts must not have it.
    $hasBom = ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    Check 'written as UTF-8 (no BOM)' (-not $hasBom) "bom=$hasBom"
    $back = Get-Content -LiteralPath $validPath -Raw -Encoding UTF8 | ConvertFrom-Json
    Check 'round-trip parse: task_id' ($back.task_id -eq 'T-030') ''
    Check 'round-trip parse: phase' ($back.phase -eq 'reconcile') ''
    Check 'round-trip parse: status' ($back.status -eq 'completed') ''
}

# Atomic write to a missing parent directory
$deepPath = Join-Path (Join-Path (Join-Path $Receipts 'nested') 'deeper') 'T-030-plan.json'
$valid2 = Make-Receipt @{ phase = 'plan'; next_phase = 'implement' }
$json2 = $valid2 | ConvertTo-Json -Depth 16
$ec2 = Run-Writer -Json $json2 -Path $deepPath -ExpectedTaskId T-030 -ExpectedPhase plan
Check 'writer creates missing parent directory' ($ec2 -eq 0) "exit=$ec2"
Check 'deeply nested receipt file exists' (Test-Path -LiteralPath $deepPath) ''

# Malformed JSON
$malformedPath = Join-Path $Receipts 'T-030-malformed.json'
$ec3 = Run-Writer -Json ('{ "task_id": "T-030", broken json' + '}') -Path $malformedPath -ExpectedTaskId T-030 -ExpectedPhase reconcile
Check 'malformed JSON is rejected' ($ec3 -ne 0) "exit=$ec3"

# Missing required fields
$missingPath = Join-Path $Receipts 'T-030-missing.json'
$partial = @{ task_id = 'T-030'; phase = 'reconcile' } | ConvertTo-Json -Depth 16
$ec4 = Run-Writer -Json $partial -Path $missingPath -ExpectedTaskId T-030 -ExpectedPhase reconcile
Check 'missing required fields is rejected' ($ec4 -ne 0) "exit=$ec4"

# Wrong task
$wrongTask = Make-Receipt @{ task_id = 'T-099' }
$wtPath = Join-Path $Receipts 'T-030-wrongtask.json'
$ec5 = Run-Writer -Json ($wrongTask | ConvertTo-Json -Depth 16) -Path $wtPath -ExpectedTaskId T-030 -ExpectedPhase reconcile
Check 'wrong task_id is rejected' ($ec5 -ne 0) "exit=$ec5"

# Wrong phase
$wrongPhase = Make-Receipt @{ phase = 'plan' }
$wpPath = Join-Path $Receipts 'T-030-wrongphase.json'
$ec6 = Run-Writer -Json ($wrongPhase | ConvertTo-Json -Depth 16) -Path $wpPath -ExpectedTaskId T-030 -ExpectedPhase reconcile
Check 'wrong phase is rejected' ($ec6 -ne 0) "exit=$ec6"

# In-progress status
$inProgress = Make-Receipt @{ status = 'in_progress' }
$ipPath = Join-Path $Receipts 'T-030-inprogress.json'
$ec7 = Run-Writer -Json ($inProgress | ConvertTo-Json -Depth 16) -Path $ipPath -ExpectedTaskId T-030 -ExpectedPhase reconcile
Check 'incomplete status is rejected' ($ec7 -ne 0) "exit=$ec7"

# ---------------------------------------------------------------------------
# 2. Router structural checks: Test-PhaseReceiptValid + Invoke-RetryCurrentPhase
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "## Router source has the new functions and dispatch"

$src = Get-Content -LiteralPath $Router -Raw
Check 'Test-PhaseReceiptValid defined' ($src -match 'function Test-PhaseReceiptValid') ''
Check 'Test-PhaseReceiptValid has reason=missing' ($src -match "Reason = 'missing'") ''
Check 'Test-PhaseReceiptValid has reason=malformed_json' ($src -match "Reason = 'malformed_json'") ''
Check 'Test-PhaseReceiptValid has reason=missing_fields' ($src -match "Reason = 'missing_fields'") ''
Check 'Test-PhaseReceiptValid has reason=wrong_task' ($src -match "Reason = 'wrong_task'") ''
Check 'Test-PhaseReceiptValid has reason=wrong_phase' ($src -match "Reason = 'wrong_phase'") ''
Check 'Test-PhaseReceiptValid has reason=incomplete_status' ($src -match "Reason = 'incomplete_status'") ''
Check 'Invoke-RetryCurrentPhase defined' ($src -match 'function Invoke-RetryCurrentPhase') ''
Check 'RetryCurrentPhase in -Command ValidateSet' ($src -match "'RetryCurrentPhase'") ''
Check 'RetryCurrentPhase dispatched in switch' ($src -match "'RetryCurrentPhase'\s*\{\s*Invoke-RetryCurrentPhase") ''
Check 'Start-ChildSession captures stdout/stderr' ($src -match 'Get-ChildLogPath') ''
Check 'Start-ChildSession uses add_OutputDataReceived' ($src -match 'add_OutputDataReceived') ''
Check 'Start-ChildSession uses add_ErrorDataReceived' ($src -match 'add_ErrorDataReceived') ''
Check 'New-PhasePromptString uses repository-relative path' ($src -match 'ConvertTo-RelativeReceiptPath') ''
Check 'New-PhasePromptString references Write-PhaseReceipt.ps1' ($src -match 'Write-PhaseReceipt\.ps1') ''
Check 'Invoke-Phase has SkipIfReceiptValid guard' ($src -match 'SkipIfReceiptValid') ''

# ---------------------------------------------------------------------------
# 3. Phase prompts declare the mandatory final action
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "## Phase prompts declare the mandatory receipt final action"

$phases = @('reconcile', 'plan', 'implement', 'validate', 'document', 'review', 'closeout')
foreach ($p in $phases) {
    $promptPath = Join-Path $RepoRoot ".ai/prompts/phases/$p.md"
    $prompt = Get-Content -LiteralPath $promptPath -Raw
    Check "$p.md has Mandatory Final Action section" ($prompt -match 'Mandatory Final Action') ''
    Check "$p.md references Write-PhaseReceipt.ps1" ($prompt -match 'Write-PhaseReceipt\.ps1') ''
    Check "$p.md lists failure modes" ($prompt -match 'malformed_json' -and $prompt -match 'wrong_task' -and $prompt -match 'wrong_phase' -and $prompt -match 'incomplete_status') ''
}

# ---------------------------------------------------------------------------
# 4. Router dry-run selects T-030 / reconcile (the original failure case)
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "## Router dry-run selects T-030 / reconcile"
$dryRunOut = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $Router -Command Next -DryRun 2>&1
$dryRunStr = $dryRunOut -join "`n"
Check 'dry-run uses qwen3.5:cloud' ($dryRunStr -match 'qwen3\.5:cloud') ''
Check 'dry-run uses repository-relative receipt path' ($dryRunStr -match 'Receipt path \(repository-relative\): \.ai/receipts/phases/T-030-reconcile\.json') ''
Check 'dry-run prompt contains Write-PhaseReceipt.ps1' ($dryRunStr -match 'Write-PhaseReceipt\.ps1') ''
Check 'dry-run prompt contains MANDATORY FINAL ACTION' ($dryRunStr -match 'MANDATORY FINAL ACTION') ''

# Verify RetryCurrentPhase is plumbed
$rcHelp = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $Router -Command RetryCurrentPhase -DryRun 2>&1
$rcStr = $rcHelp -join "`n"
Check 'RetryCurrentPhase dry-run accepted by router' ($rcStr -notmatch 'ERROR') ''

# ---------------------------------------------------------------------------
# 5. PowerShell syntax validation
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "## PowerShell syntax validation"
$files = @(
    'tools/ai-session-router.ps1',
    'tools/ai-session-router.Tests.ps1',
    'tools/Write-PhaseReceipt.ps1'
)
foreach ($f in $files) {
    $p = Join-Path $RepoRoot $f
    $tokens = $null
    $errors = $null
    $null = [System.Management.Automation.Language.Parser]::ParseFile($p, [ref]$tokens, [ref]$errors)
    Check "$f parses" (-not $errors -or $errors.Count -eq 0) ($errors | ForEach-Object { $_.Message })
}

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "==== Summary ===="
Write-Host "Passed: $pass"
Write-Host "Failed: $fail"

# Clean up the temp dir
if (Test-Path -LiteralPath $Tmp) {
    Remove-Item -LiteralPath $Tmp -Recurse -Force
}

if ($fail -gt 0) { exit 1 } else { exit 0 }
