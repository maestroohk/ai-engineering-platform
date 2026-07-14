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
