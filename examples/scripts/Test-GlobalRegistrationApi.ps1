# Test-GlobalRegistrationApi.ps1 — integration tests for GlobalRegistrationApi /api/users

. "$PSScriptRoot\Shared\ApiTestHelpers.ps1"

$base        = "http://localhost:5010"
$aliceId     = "11111111-1111-1111-1111-111111111111"
$bobId       = "22222222-2222-2222-2222-222222222222"
$notFoundId  = "99999999-9999-9999-9999-999999999999"

Initialize-Suite "GlobalRegistrationApi — /api/users"

if (-not (Test-ApiReachable -BaseUrl $base -ApiName "GlobalRegistrationApi")) {
    exit 0
}

# ---------------------------------------------------------------------------
# Section 1 — GET all users
# ---------------------------------------------------------------------------
Write-TestHeader "GET /api/users"
$r = Invoke-ApiRequest GET "$base/api/users"
Assert-StatusCode $r 200 "GET all -> 200"
Assert-NotNull $r.Data "GET all -> body is not null"
Assert-Value ($r.Data.Count -ge 2) $true "GET all -> returns at least 2 users"

# ---------------------------------------------------------------------------
# Section 2 — GET by ID
# ---------------------------------------------------------------------------
Write-TestHeader "GET /api/users/{id}"

# valid id — Alice
$r = Invoke-ApiRequest GET "$base/api/users/$aliceId"
Assert-StatusCode $r 200 "GET Alice -> 200"
Assert-Value $r.Data.name "Alice" "GET Alice -> name is Alice"
Assert-Value $r.Data.bio "Software engineer" "GET Alice -> bio is Software engineer"

# valid id — Bob (null bio)
$r = Invoke-ApiRequest GET "$base/api/users/$bobId"
Assert-StatusCode $r 200 "GET Bob -> 200"
Assert-Null $r.Data.bio "GET Bob -> bio is null"

# not found
$r = Invoke-ApiRequest GET "$base/api/users/$notFoundId"
Assert-StatusCode $r 404 "GET unknown id -> 404"

# ---------------------------------------------------------------------------
# Section 3 — PATCH: update name only
# ---------------------------------------------------------------------------
Write-TestHeader "PATCH /api/users/{id} — update name only"
$r = Invoke-ApiRequest PATCH "$base/api/users/$aliceId" @{ name = "Alice Updated" }
Assert-StatusCode $r 200 "PATCH name only -> 200"
Assert-Value $r.Data.name "Alice Updated" "PATCH name only -> name changed"
Assert-Value $r.Data.bio "Software engineer" "PATCH name only -> bio unchanged"

# ---------------------------------------------------------------------------
# Section 4 — PATCH: clear bio (send null)
# NOTE: Use PSCustomObject + ConvertTo-Json to guarantee "bio": null in the JSON.
#       A hashtable with $null value would omit the field entirely.
# ---------------------------------------------------------------------------
Write-TestHeader "PATCH /api/users/{id} — clear bio (null)"
$body = [PSCustomObject]@{ bio = $null }
$bodyJson = $body | ConvertTo-Json
$params = @{
    Method             = 'PATCH'
    Uri                = "$base/api/users/$aliceId"
    ContentType        = 'application/json'
    Body               = $bodyJson
    SkipHttpErrorCheck = $true
}
$response = Invoke-WebRequest @params
$data = $response.Content | ConvertFrom-Json
$r2 = [PSCustomObject]@{
    StatusCode = [int]$response.StatusCode
    Data       = $data
    IsSuccess  = $response.StatusCode -lt 400
}
Assert-StatusCode $r2 200 "PATCH bio=null -> 200"
Assert-Null $r2.Data.bio "PATCH bio=null -> bio is now null"

# ---------------------------------------------------------------------------
# Section 5 — PATCH: update both fields
# ---------------------------------------------------------------------------
Write-TestHeader "PATCH /api/users/{id} — update both fields"
$r = Invoke-ApiRequest PATCH "$base/api/users/$aliceId" @{ name = "Alice Final"; bio = "Updated bio" }
Assert-StatusCode $r 200 "PATCH both -> 200"
Assert-Value $r.Data.name "Alice Final" "PATCH both -> name updated"
Assert-Value $r.Data.bio "Updated bio" "PATCH both -> bio updated"

# ---------------------------------------------------------------------------
# Section 6 — PATCH: empty body (no fields specified)
# ---------------------------------------------------------------------------
Write-TestHeader "PATCH /api/users/{id} — empty body"
$before = Invoke-ApiRequest GET "$base/api/users/$aliceId"
$r = Invoke-ApiRequest PATCH "$base/api/users/$aliceId" @{}
Assert-StatusCode $r 200 "PATCH empty -> 200"
Assert-Value $r.Data.name $before.Data.name "PATCH empty -> name unchanged"
Assert-Value $r.Data.bio $before.Data.bio "PATCH empty -> bio unchanged"

# ---------------------------------------------------------------------------
# Section 7 — PATCH: not found
# ---------------------------------------------------------------------------
Write-TestHeader "PATCH /api/users/{id} — not found"
$r = Invoke-ApiRequest PATCH "$base/api/users/$notFoundId" @{ name = "Ghost" }
Assert-StatusCode $r 404 "PATCH unknown id -> 404"

# ---------------------------------------------------------------------------
# Results
# ---------------------------------------------------------------------------
$failures = Complete-Suite
exit $failures
