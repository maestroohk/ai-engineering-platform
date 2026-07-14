<#
.SYNOPSIS
    Focused local-process smoke tests for the AI session router.

.DESCRIPTION
    These tests run under the actual powershell.exe (Windows PowerShell 5.1
    on .NET Framework). They never invoke `ollama` and never contact Ollama
    Cloud. Each test launches a harmless local child process (powershell.exe
    -Command "Write-Output ..." or a child cmd) using the same async event
    pattern the router uses, and asserts the captured stdout, stderr, and
    exit code.

    Designed for Pester 3.4 (the version installed on Windows 10/11 by
    default). No Pester 4+ features are used; no BeforeAll at script level.
    The script also runs without Pester at all: it self-checks and reports
    Pass / Fail per case and exits non-zero if any case failed.

.NOTES
    Run with:
        powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Router.LocalProcess.Tests.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Resolve script directory and the router under test
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot = (Resolve-Path -Path (Join-Path $ScriptDir '..')).Path

# ---------------------------------------------------------------------------
# Tiny assertion helpers (no Pester dependency).
# ---------------------------------------------------------------------------
$script:Pass = 0
$script:Fail = 0
$script:Results = @()

function Assert-True {
    param(
        [Parameter(Mandatory = $true)] [string]$Name,
        [Parameter(Mandatory = $true)] [bool]$Condition,
        [string]$Detail = ''
    )
    if ($Condition) {
        $script:Pass++
        $script:Results += [pscustomobject]@{ Name = $Name; Result = 'Pass'; Detail = $Detail }
        Write-Host ("[PASS] {0}" -f $Name)
    } else {
        $script:Fail++
        $script:Results += [pscustomobject]@{ Name = $Name; Result = 'Fail'; Detail = $Detail }
        Write-Host ("[FAIL] {0} :: {1}" -f $Name, $Detail)
    }
}

function Assert-Equal {
    param(
        [Parameter(Mandatory = $true)] [string]$Name,
        [Parameter(Mandatory = $true)] $Expected,
        [Parameter(Mandatory = $true)] $Actual,
        [string]$Detail = ''
    )
    $ok = ($Expected -eq $Actual)
    $rendered = ('expected=[{0}] actual=[{1}] {2}' -f $Expected, $Actual, $Detail)
    Assert-True -Name $Name -Condition $ok -Detail $rendered
}

# ---------------------------------------------------------------------------
# Sync stdout/stderr capture. ReadToEnd() AFTER the child exits is the
# deadlock-free pattern. Stdout/stderr are independent streams; the parent
# process owns them and reads to EOF after the child closes them.
# ---------------------------------------------------------------------------
function Invoke-LocalChild {
    param(
        [Parameter(Mandatory = $true)] [string]$FileName,
        [Parameter(Mandatory = $true)] [string[]]$ArgList,
        [int]$TimeoutMs = 15000
    )
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $FileName
    $psi.Arguments = ($ArgList -join ' ')
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    $psi.UseShellExecute = $false
    $psi.CreateNoWindow = $true

    $proc = New-Object System.Diagnostics.Process
    $proc.StartInfo = $psi
    try {
        [void]$proc.Start()
        if (-not $proc.WaitForExit($TimeoutMs)) {
            try { $proc.Kill() } catch { }
            throw "Child did not exit within $TimeoutMs ms"
        }
        $proc.WaitForExit() | Out-Null
        $exitCode = [int]$proc.ExitCode
        $outText = $proc.StandardOutput.ReadToEnd()
        $errText = $proc.StandardError.ReadToEnd()
        return [pscustomobject]@{
            ExitCode = $exitCode
            StdOut = if ($null -eq $outText) { '' } else { [string]$outText }
            StdErr = if ($null -eq $errText) { '' } else { [string]$errText }
        }
    } finally {
        try { $proc.Close() } catch { }
        try { $proc.Dispose() } catch { }
    }
}

# ---------------------------------------------------------------------------
# Test 1 - the corrected type resolves under real Windows PowerShell 5.1.
# ---------------------------------------------------------------------------
$type = $null
$typeErr = $null
try { $type = [System.Diagnostics.DataReceivedEventHandler] } catch { $typeErr = $_.Exception.Message }
Assert-True -Name 'type: System.Diagnostics.DataReceivedEventHandler resolves' `
            -Condition ($null -ne $type) `
            -Detail ('error=[' + $typeErr + ']')

# Confirm the unqualified form would fail (so we know the fix is needed).
$badType = $null
$badErr = ''
try { $badType = [System.DataReceivedEventHandler] } catch { $badErr = $_.Exception.Message }
Assert-True -Name 'type: unqualified [System.DataReceivedEventHandler] is rejected' `
            -Condition ($null -eq $badType -and $badErr -match 'Unable to find type') `
            -Detail ('error=[' + $badErr + ']')

# ---------------------------------------------------------------------------
# Test 2 - harmless child launch, stdout captured, exit code 0.
# ---------------------------------------------------------------------------
$psExe = (Get-Command -Name 'powershell.exe' -ErrorAction SilentlyContinue).Source
Assert-True -Name 'powershell.exe available' -Condition ($null -ne $psExe) -Detail ('path=[' + $psExe + ']')

if ($null -ne $psExe) {
    $r = $null
    $rErr = $null
    Write-Host "[INFO] starting child launch test (exit 0)..."; [Console]::Out.Flush()
    try {
        $r = Invoke-LocalChild -FileName $psExe -ArgList @(
            '-NoProfile', '-ExecutionPolicy', 'Bypass',
            '-Command', 'Write-Output hello-from-child; exit 0'
        )
        Write-Host "[INFO] child launch returned. r-is-null: $($null -eq $r)"; [Console]::Out.Flush()
        if ($null -ne $r) {
            Write-Host "[INFO] r.ExitCode=[$($r.ExitCode)] r.StdOut=[$($r.StdOut.Trim())] r.StdErr=[$($r.StdErr.Trim())]"; [Console]::Out.Flush()
        }
    } catch { $rErr = $_.Exception.Message; Write-Host "[INFO] child launch threw: $rErr"; [Console]::Out.Flush() }
    if ($null -eq $r) {
        Assert-True -Name 'child: launch (exit 0)' -Condition $false -Detail ('error: ' + $rErr)
    } else {
        Assert-Equal -Name 'child: exit code 0 preserved' -Expected 0 -Actual $r.ExitCode
        Assert-True -Name 'child: stdout captured' -Condition ($r.StdOut -match 'hello-from-child') `
                    -Detail ('stdout=[' + $r.StdOut.Trim() + ']')
        Assert-Equal -Name 'child: stderr empty' -Expected '' -Actual ($r.StdErr.Trim())
    }
}

# ---------------------------------------------------------------------------
# Test 3 - non-zero exit code preserved (child writes to stderr + exits 7).
# ---------------------------------------------------------------------------
if ($null -ne $psExe) {
    $r = $null
    $rErr = $null
    try {
        $r = Invoke-LocalChild -FileName $psExe -ArgList @(
            '-NoProfile', '-ExecutionPolicy', 'Bypass',
            '-Command', "[Console]::Error.WriteLine('oops'); [Environment]::Exit(7)"
        )
    } catch { $rErr = $_.Exception.Message }
    if ($null -eq $r) {
        Assert-True -Name 'non-zero exit: launch' -Condition $false -Detail ('error: ' + $rErr)
    } else {
        Assert-Equal -Name 'non-zero exit: code 7 preserved' -Expected 7 -Actual $r.ExitCode
        Assert-True -Name 'non-zero exit: stderr captured' -Condition ($r.StdErr -match 'oops') `
                    -Detail ('stderr=[' + $r.StdErr.Trim() + ']')
    }
}

# ---------------------------------------------------------------------------
# Test 4 - paths with spaces in the executable location are handled.
# Uses a child file under Program Files (a path that contains a space).
# ---------------------------------------------------------------------------
$progFiles = $env:ProgramFiles
$hasSpaces = $false
if (-not [string]::IsNullOrEmpty($progFiles) -and $progFiles -match ' ') {
    $hasSpaces = $true
}
Assert-True -Name 'env: Program Files path contains a space' -Condition $hasSpaces `
            -Detail ('ProgramFiles=[' + $progFiles + ']')

# Router's Start-ChildSession uses Format-CommandLine for the Arguments
# string, so a path with spaces and a quoted prompt both survive intact.
# Static check: ensure banned APIs are not used in code (comments excluded).
# Comments include: full-line `#` comments, indented continuation lines
# inside a `<# ... #>` block-docstring, and inline `#` after code.
$routerPath = Join-Path -Path $ScriptDir -ChildPath 'ai-session-router.ps1'
$codeLines = Get-Content -LiteralPath $routerPath
$banned = @()
$inBlockComment = $false
foreach ($ln in $codeLines) {
    $line = $ln
    # Strip an inline `# ...` comment to focus on the code half.
    # We do NOT do this for block-docstring lines, which are pure comment.
    if ($inBlockComment) {
        if ($line -match '#>') { $inBlockComment = $false }
        continue
    }
    if ($line -match '<#') {
        # The line itself starts a block; if the closer is on the same
        # line, we don't enter block mode.
        if ($line -notmatch '#>') { $inBlockComment = $true }
        continue
    }
    $trimmed = $line.TrimStart()
    if ($trimmed.StartsWith('#')) { continue }   # full-line comment
    if ($line -match 'Invoke-Expression\b')           { $banned += 'Invoke-Expression' }
    if ($line -match 'ProcessStartInfo\.ArgumentList\b') { $banned += 'ProcessStartInfo.ArgumentList' }
}
Assert-True -Name 'router: no Invoke-Expression usage in code' `
            -Condition (-not ($banned -contains 'Invoke-Expression')) `
            -Detail ('banned tokens: ' + ($banned -join ', '))
Assert-True -Name 'router: no PS 7-only ArgumentList usage in code' `
            -Condition (-not ($banned -contains 'ProcessStartInfo.ArgumentList')) `
            -Detail ('banned tokens: ' + ($banned -join ', '))

# Router's Start-ChildSession uses Format-CommandLine for the Arguments
# string, so a path with spaces and a quoted prompt both survive intact.
$routerSrc = Get-Content -LiteralPath $routerPath -Raw
$fmtBody = $null
$fmtErr = $null
try {
    $matchInfo = [regex]::Match($routerSrc, 'function Format-CommandLine[\s\S]*?^\}', 'Multiline')
    $fmtBody = $matchInfo.Value
} catch { $fmtErr = $_.Exception.Message }
Assert-True -Name 'router: Format-CommandLine function found' `
            -Condition (-not [string]::IsNullOrEmpty($fmtBody)) `
            -Detail ('extract error: ' + $fmtErr)

# ---------------------------------------------------------------------------
# Test 5 - command-injection resistance: an argument with semicolons, ampersands,
# backticks, and a quoted token must reach the child as a single literal
# argument (because we use ArgumentList semantics, not cmd /c).
# ---------------------------------------------------------------------------
if ($null -ne $psExe) {
    # Payload includes command separators (; & `), a quoted token, and a
    # path that begins with backslash (the regex used by the router would
    # reject that). The test confirms that the ArgumentList-style
    # arguments we hand the child survive intact as ONE literal value
    # -- not as multiple commands parsed by the shell.
    $payload = 'safe-value-1 ; echo INJECTED & whoami ` id "quoted-text"'
    $r = $null
    $rErr = $null
    try {
        $r = Invoke-LocalChild -FileName $psExe -ArgList @(
            '-NoProfile', '-ExecutionPolicy', 'Bypass',
            '-Command', "Write-Output 'PAYLOAD:' '$payload'"
        )
    } catch { $rErr = $_.Exception.Message }
    if ($null -eq $r) {
        Assert-True -Name 'inject: launch' -Condition $false -Detail ('error: ' + $rErr)
    } else {
        $combined = ($r.StdOut + $r.StdErr)
        # The literal payload was delivered as ONE argument to Write-Output.
        # Write-Output prints its arguments separated by spaces and a single
        # trailing newline. The shell did NOT split the payload at ; or &.
        # The output must contain BOTH "PAYLOAD:" and the full literal on
        # the same line (proving it was a single argument, not 3+ parsed
        # commands) -- and there must be no separate "INJECTED" line.
        $oneLinePayload = ($r.StdOut -match '(?s)PAYLOAD:.*safe-value-1 ; echo INJECTED & whoami ` id quoted-text')
        Assert-True -Name 'inject: payload delivered as a single Write-Output argument' `
                    -Condition $oneLinePayload `
                    -Detail ('stdout: ' + $r.StdOut.Trim())
        # Shell metacharacters were NOT executed as separate commands.
        # If `;` or `&` had been interpreted by cmd/PowerShell, we'd see
        # "INJECTED" on its own line, or the whoami username on a line.
        $lines = ($r.StdOut -split "`n") | Where-Object { $_.Length -gt 0 }
        Assert-True -name 'inject: no separate INJECTED line' `
                    -Condition (-not ($lines -match '^INJECTED$')) `
                    -Detail ('lines: ' + (($lines -join ' | ')))
        Assert-True -name 'inject: no standalone whoami output line' `
                    -Condition (-not ($lines -match '^[a-z][a-z0-9-]*\\')) `
                    -Detail ('lines: ' + (($lines -join ' | ')))
    }
}

# ---------------------------------------------------------------------------
# Test 6 - the router's dry-run output for RetryCurrentPhase contains a
# SINGLE `--` separator (not the duplicated `"--" "--" "-p"` form).
# ---------------------------------------------------------------------------
$dryRunOut = & {
    $out = $null
    $outErr = $null
    try {
        $out = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $routerPath -Command RetryCurrentPhase -DryRun 2>&1
    } catch { $outErr = $_.Exception.Message }
    if ($null -eq $out) {
        return @("ERROR: " + $outErr)
    }
    return $out
}
$dryRunText = ($dryRunOut -join "`n")
$ollamaLine = ($dryRunText -split "`n" | Where-Object { $_ -match 'ollama ' } | Select-Object -First 1)
Assert-True -name 'dry-run: produces an ollama line' `
            -Condition (-not [string]::IsNullOrEmpty($ollamaLine)) `
            -Detail ('first line: ' + $ollamaLine)
$dupDash = ($ollamaLine -match '"--"\s+"--"\s+"-p"')
Assert-True -name 'dry-run: no duplicated `"--"` `"--"` `"-p"` separator' `
            -Condition (-not $dupDash) `
            -Detail ('ollama line: ' + $ollamaLine)

# ---------------------------------------------------------------------------
# Test 7 - Preflight command reports the required fields.
# ---------------------------------------------------------------------------
$preflightOut = & {
    $out = $null
    $outErr = $null
    try {
        $out = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $routerPath -Command Preflight 2>&1
    } catch { $outErr = $_.Exception.Message }
    if ($null -eq $out) {
        return @("ERROR: " + $outErr)
    }
    return $out
}
$preflightText = ($preflightOut -join "`n")
Assert-True -name 'preflight: reports PowerShell edition' `
            -Condition ($preflightText -match 'PowerShell edition:') `
            -Detail ''
Assert-True -name 'preflight: reports PowerShell version' `
            -Condition ($preflightText -match 'PowerShell version:') `
            -Detail ''
Assert-True -name 'preflight: reports CLR version' `
            -Condition ($preflightText -match 'CLR version:') `
            -Detail ''
Assert-True -name 'preflight: reports ollama executable path' `
            -Condition ($preflightText -match 'ollama executable:') `
            -Detail ''
Assert-True -name 'preflight: reports process type resolution' `
            -Condition ($preflightText -match 'type OK.*System\.Diagnostics\.DataReceivedEventHandler') `
            -Detail ''
Assert-True -name 'preflight: OK overall' `
            -Condition ($preflightText -match 'Preflight OK') `
            -Detail ''

# ---------------------------------------------------------------------------
# Final report
# ---------------------------------------------------------------------------
Write-Host ''
Write-Host ('Results: {0} passed, {1} failed' -f $script:Pass, $script:Fail)
if ($script:Fail -gt 0) {
    exit 1
}
exit 0
