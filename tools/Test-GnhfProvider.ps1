#requires -Version 5.1
<#
.SYNOPSIS
    Smoke test for the GNHF provider (T-032).

.DESCRIPTION
    Runs the AiEng.Platform.Providers.Gnhf.SmokeTest program
    against a local gnhf executable (or stand-in). Does not
    modify the upstream gnhf clone. Exits 0 on a successful
    probe (Available=True with a parsed version), 1 on a
    failed probe, and 2 on usage error.

.PARAMETER Executable
    Path to the gnhf binary (or stand-in). On Windows the
    default is "gnhf.cmd" (resolved via PATH).

.PARAMETER SimulateUnavailable
    Run the smoke test in the always-failing-process-runner
    mode to verify the unavailable code path.

.EXAMPLE
    powershell.exe -NoProfile -File tools\Test-GnhfProvider.ps1
    # Uses PATH lookup for "gnhf.cmd".

.EXAMPLE
    powershell.exe -NoProfile -File tools\Test-GnhfProvider.ps1 -Executable "C:\path\to\gnhf.cmd"

.EXAMPLE
    powershell.exe -NoProfile -File tools\Test-GnhfProvider.ps1 -SimulateUnavailable
#>

[CmdletBinding()]
param(
    [string] $Executable,
    [switch] $SimulateUnavailable
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path "$PSScriptRoot\..").Path
$smokeTestProject = Join-Path $repoRoot "tools\GnhfSmokeTest\GnhfSmokeTest.csproj"

if (-not (Test-Path $smokeTestProject)) {
    Write-Error "GnhfSmokeTest project not found at $smokeTestProject"
    exit 2
}

if (-not $Executable) {
    $cmd = Get-Command gnhf.cmd -ErrorAction SilentlyContinue
    if ($cmd) {
        $Executable = $cmd.Path
    } else {
        $gnu = Get-Command gnhf -ErrorAction SilentlyContinue
        if ($gnu) {
            $Executable = $gnu.Path
        } else {
            Write-Error "No gnhf binary on PATH. Provide -Executable or install gnhf (npm i -g gnhf)."
            exit 2
        }
    }
}

if (-not (Test-Path $Executable)) {
    Write-Error "Executable not found: $Executable"
    exit 2
}

$arguments = @($Executable)
if ($SimulateUnavailable) {
    $arguments += "--simulate-unavailable"
}

Write-Host "Running GNHF provider smoke test..."
Write-Host "  Executable : $Executable"
Write-Host "  Mode       : $(if ($SimulateUnavailable) { 'simulate-unavailable' } else { 'real' })"

$output = & dotnet run --project $smokeTestProject --no-build -- $arguments 2>&1
$exit = $LASTEXITCODE

$output | ForEach-Object { Write-Host "  $_" }

if ($exit -ne 0) {
    Write-Error "Smoke test failed with exit code $exit."
    exit $exit
}

Write-Host "Smoke test OK."
exit 0
