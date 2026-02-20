# ApiTestHelpers.ps1 — shared test infrastructure for API integration tests

$script:Passed = 0
$script:Failed = 0
$script:CurrentSuite = ""

function Initialize-Suite {
    param([string]$Name)
    $script:Passed = 0
    $script:Failed = 0
    $script:CurrentSuite = $Name
    Write-Host ""
    Write-Host "============================================================"
    Write-Host "  SUITE: $Name"
    Write-Host "============================================================"
}

function Write-TestHeader {
    param([string]$Title)
    Write-Host ""
    Write-Host "--- $Title ---"
}

function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Url,
        [hashtable]$Body = $null
    )
    try {
        $iwrParams = @{
            Method             = $Method
            Uri                = $Url
            ContentType        = 'application/json'
            SkipHttpErrorCheck = $true
        }
        if ($null -ne $Body) {
            $iwrParams['Body'] = ($Body | ConvertTo-Json -Depth 10)
        }
        $response = Invoke-WebRequest @iwrParams
        $parsedData = $null
        if ($response.Content) {
            try { $parsedData = $response.Content | ConvertFrom-Json } catch { }
        }
        return [PSCustomObject]@{
            StatusCode = [int]$response.StatusCode
            Data       = $parsedData
            Raw        = $response.Content
            IsSuccess  = $response.StatusCode -lt 400
        }
    }
    catch {
        Write-Host "  [ERROR] $Method $Url - $_"
        return [PSCustomObject]@{
            StatusCode = 0
            Data       = $null
            Raw        = ""
            IsSuccess  = $false
        }
    }
}

function Assert-StatusCode {
    param(
        [PSCustomObject]$Result,
        [int]$Expected,
        [string]$TestName
    )
    if ($Result.StatusCode -eq $Expected) {
        $script:Passed++
        Write-Host "  [PASS] $TestName"
    }
    else {
        $script:Failed++
        Write-Host "  [FAIL] $TestName  (expected $Expected, got $($Result.StatusCode))"
    }
}

function Assert-Value {
    param(
        $Actual,
        $Expected,
        [string]$TestName
    )
    if ([string]$Actual -eq [string]$Expected) {
        $script:Passed++
        Write-Host "  [PASS] $TestName"
    }
    else {
        $script:Failed++
        Write-Host "  [FAIL] $TestName  (expected '$Expected', got '$Actual')"
    }
}

function Assert-Null {
    param($Value, [string]$TestName)
    if ($Value -eq $null) {
        $script:Passed++
        Write-Host "  [PASS] $TestName"
    }
    else {
        $script:Failed++
        Write-Host "  [FAIL] $TestName  (expected null, got '$Value')"
    }
}

function Assert-NotNull {
    param($Value, [string]$TestName)
    if ($Value -ne $null) {
        $script:Passed++
        Write-Host "  [PASS] $TestName"
    }
    else {
        $script:Failed++
        Write-Host "  [FAIL] $TestName  (expected non-null)"
    }
}

function Complete-Suite {
    Write-Host ""
    Write-Host "============================================================"
    Write-Host "  RESULTS: $script:Passed passed, $script:Failed failed"
    Write-Host "============================================================"
    Write-Host ""
    return $script:Failed
}

function Test-ApiReachable {
    param([string]$BaseUrl, [string]$ApiName)
    $result = Invoke-ApiRequest -Method GET -Url $BaseUrl
    if ($result.StatusCode -gt 0) {
        return $true
    }
    else {
        Write-Host "  [SKIP] $ApiName is not running at $BaseUrl — start it with: dotnet run"
        return $false
    }
}
