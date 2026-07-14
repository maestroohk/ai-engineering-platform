<#
.SYNOPSIS
    AI session router — runnable example. Copy to ai-session-router.ps1
    and edit. See the production script for the canonical implementation.

.DESCRIPTION
    This file exists so the user has a starting point without committing
    custom changes to the production router. Copy it next to the production
    script and edit your local copy.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Next', 'Resume', 'Finish', 'Status', 'Plan', 'Configure', 'DryRun')]
    [string]$Command
)

Write-Host "This is the example router. Copy it to ai-session-router.ps1 and replace this body with your customisation." -ForegroundColor Yellow
Write-Host "See tools/ai-session-router.ps1 for the production implementation." -ForegroundColor Yellow
