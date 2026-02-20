#Requires -Version 7
<#
.SYNOPSIS
    Runs all API integration tests.

.DESCRIPTION
    Executes Test-GlobalRegistrationApi.ps1 and Test-PerPropertyAttributeApi.ps1 in sequence.
    APIs that are not running are skipped (exit 0), not failed.

.EXAMPLE
    # Start both APIs first (two separate terminals):
    #   dotnet run --project examples/GlobalRegistrationApi
    #   dotnet run --project examples/PerPropertyAttributeApi
    #
    # Then run all tests:
    pwsh examples/scripts/Test-AllApis.ps1
#>

$ErrorActionPreference = 'Stop'
$scriptDir = $PSScriptRoot

$scripts = @(
    "$scriptDir\Test-GlobalRegistrationApi.ps1",
    "$scriptDir\Test-PerPropertyAttributeApi.ps1"
)

$totalFailures = 0

foreach ($script in $scripts) {
    Write-Host ""
    Write-Host "###############################################################"
    Write-Host "  Running: $(Split-Path $script -Leaf)"
    Write-Host "###############################################################"

    & $script
    $totalFailures += $LASTEXITCODE
}

Write-Host ""
Write-Host "###############################################################"
if ($totalFailures -eq 0) {
    Write-Host "  ALL TESTS PASSED"
} else {
    Write-Host "  TOTAL FAILURES: $totalFailures"
}
Write-Host "###############################################################"
Write-Host ""

exit $totalFailures
