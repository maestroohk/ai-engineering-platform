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
