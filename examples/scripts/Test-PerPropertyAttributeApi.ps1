#Requires -Version 7
. "$PSScriptRoot\Shared\ApiTestHelpers.ps1"

$base = "http://localhost:5000"

# --- Reachability check ---
if (-not (Test-ApiReachable -BaseUrl $base -ApiName "PerPropertyAttributeApi")) {
    exit 0
}

# --- Known IDs ---
$idAaaa    = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
$idBbbb    = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
$idCccc    = "cccccccc-cccc-cccc-cccc-cccccccccccc"
$idDddd    = "dddddddd-dddd-dddd-dddd-dddddddddddd"
$idEeee    = "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"
$idFfff    = "ffffffff-ffff-ffff-ffff-ffffffffffff"
$id1111    = "11111111-2222-3333-4444-555555555555"
$id6666    = "66666666-7777-8888-9999-aaaaaaaaaaaa"
$idMissing = "99999999-9999-9999-9999-999999999999"

$totalFailures = 0

###############################################################################
# Suite 1: Primitives
###############################################################################
Initialize-Suite "PerPropertyAttributeApi — Primitives (/api/primitives)"

# 1. GET all -> 200, at least 2 items
$r = Invoke-ApiRequest -Method GET -Url "$base/api/primitives"
Assert-StatusCode $r 200 "GET /api/primitives returns 200"
Assert-NotNull $r.Data "GET /api/primitives returns data"
$itemCount = @($r.Data).Count
Assert-Value ($itemCount -ge 2) $true "GET /api/primitives returns at least 2 items"

# 2. GET by id {aaaa} -> 200, name="Draft post", isPublished=false
$r = Invoke-ApiRequest -Method GET -Url "$base/api/primitives/$idAaaa"
Assert-StatusCode $r 200 "GET /api/primitives/{aaaa} returns 200"
Assert-Value $r.Data.name "Draft post" "GET /api/primitives/{aaaa} name is 'Draft post'"
Assert-Value $r.Data.isPublished $false "GET /api/primitives/{aaaa} isPublished is false"

# 3. GET by missing id -> 404
$r = Invoke-ApiRequest -Method GET -Url "$base/api/primitives/$idMissing"
Assert-StatusCode $r 404 "GET /api/primitives/{missing} returns 404"

# 4. PATCH {name="Updated draft"} -> 200, name changed, isPublished still false
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/primitives/$idAaaa" -Body @{ name = "Updated draft" }
Assert-StatusCode $r 200 "PATCH /api/primitives/{aaaa} name only returns 200"
Assert-Value $r.Data.name "Updated draft" "PATCH /api/primitives/{aaaa} name updated to 'Updated draft'"
Assert-Value $r.Data.isPublished $false "PATCH /api/primitives/{aaaa} isPublished unchanged (still false)"

# 5. PATCH {isPublished=true} -> 200, isPublished changed, name still "Updated draft"
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/primitives/$idAaaa" -Body @{ isPublished = $true }
Assert-StatusCode $r 200 "PATCH /api/primitives/{aaaa} isPublished only returns 200"
Assert-Value $r.Data.isPublished $true "PATCH /api/primitives/{aaaa} isPublished changed to true"
Assert-Value $r.Data.name "Updated draft" "PATCH /api/primitives/{aaaa} name unchanged (still 'Updated draft')"

# 6. PATCH both {name="Final", isPublished=false} -> 200, both updated
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/primitives/$idAaaa" -Body @{ name = "Final"; isPublished = $false }
Assert-StatusCode $r 200 "PATCH /api/primitives/{aaaa} both fields returns 200"
Assert-Value $r.Data.name "Final" "PATCH /api/primitives/{aaaa} name updated to 'Final'"
Assert-Value $r.Data.isPublished $false "PATCH /api/primitives/{aaaa} isPublished updated to false"

# 7. PATCH empty {} -> 200, nothing changes
$rBefore = Invoke-ApiRequest -Method GET -Url "$base/api/primitives/$idAaaa"
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/primitives/$idAaaa" -Body @{}
Assert-StatusCode $r 200 "PATCH /api/primitives/{aaaa} empty body returns 200"
Assert-Value $r.Data.name $rBefore.Data.name "PATCH /api/primitives/{aaaa} empty body leaves name unchanged"
Assert-Value $r.Data.isPublished $rBefore.Data.isPublished "PATCH /api/primitives/{aaaa} empty body leaves isPublished unchanged"

# 8. PATCH on missing id -> 404
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/primitives/$idMissing" -Body @{ name = "Ghost" }
Assert-StatusCode $r 404 "PATCH /api/primitives/{missing} returns 404"

$totalFailures += Complete-Suite

###############################################################################
# Suite 2: Enums
###############################################################################
Initialize-Suite "PerPropertyAttributeApi — Enums (/api/enums)"

# 1. GET all -> 200, at least 2 items
$r = Invoke-ApiRequest -Method GET -Url "$base/api/enums"
Assert-StatusCode $r 200 "GET /api/enums returns 200"
Assert-NotNull $r.Data "GET /api/enums returns data"
$itemCount = @($r.Data).Count
Assert-Value ($itemCount -ge 2) $true "GET /api/enums returns at least 2 items"

# 2. GET {cccc} -> 200, priority is not null
$r = Invoke-ApiRequest -Method GET -Url "$base/api/enums/$idCccc"
Assert-StatusCode $r 200 "GET /api/enums/{cccc} returns 200"
Assert-NotNull $r.Data.priority "GET /api/enums/{cccc} priority is not null"

# 3. GET {dddd} -> 200, priority is null
$r = Invoke-ApiRequest -Method GET -Url "$base/api/enums/$idDddd"
Assert-StatusCode $r 200 "GET /api/enums/{dddd} returns 200"
Assert-Null $r.Data.priority "GET /api/enums/{dddd} priority is null"

# 4. GET missing -> 404
$r = Invoke-ApiRequest -Method GET -Url "$base/api/enums/$idMissing"
Assert-StatusCode $r 404 "GET /api/enums/{missing} returns 404"

# 5. PATCH {priority=2} (High) on {cccc} -> 200, priority becomes 2
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/enums/$idCccc" -Body @{ priority = 2 }
Assert-StatusCode $r 200 "PATCH /api/enums/{cccc} priority=2 returns 200"
Assert-Value $r.Data.priority 2 "PATCH /api/enums/{cccc} priority updated to 2 (High)"

# 6. PATCH {priority=null} on {cccc} — clear it -> 200, priority is null
$bodyJson = [PSCustomObject]@{ priority = $null } | ConvertTo-Json
$raw = Invoke-WebRequest -Method PATCH -Uri "$base/api/enums/$idCccc" -ContentType 'application/json' -Body $bodyJson -SkipHttpErrorCheck
$r = [PSCustomObject]@{ StatusCode = [int]$raw.StatusCode; Data = ($raw.Content | ConvertFrom-Json); IsSuccess = $raw.StatusCode -lt 400 }
Assert-StatusCode $r 200 "PATCH /api/enums/{cccc} priority=null returns 200"
Assert-Null $r.Data.priority "PATCH /api/enums/{cccc} priority cleared to null"

# 7. PATCH {} empty on {dddd} -> 200, priority still null
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/enums/$idDddd" -Body @{}
Assert-StatusCode $r 200 "PATCH /api/enums/{dddd} empty body returns 200"
Assert-Null $r.Data.priority "PATCH /api/enums/{dddd} empty body leaves priority null"

# 8. PATCH on missing id -> 404
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/enums/$idMissing" -Body @{ priority = 1 }
Assert-StatusCode $r 404 "PATCH /api/enums/{missing} returns 404"

$totalFailures += Complete-Suite

###############################################################################
# Suite 3: NestedObjects
###############################################################################
Initialize-Suite "PerPropertyAttributeApi — NestedObjects (/api/nestedobjects)"

# 1. GET all -> 200, at least 2 items
$r = Invoke-ApiRequest -Method GET -Url "$base/api/nestedobjects"
Assert-StatusCode $r 200 "GET /api/nestedobjects returns 200"
Assert-NotNull $r.Data "GET /api/nestedobjects returns data"
$itemCount = @($r.Data).Count
Assert-Value ($itemCount -ge 2) $true "GET /api/nestedobjects returns at least 2 items"

# 2. GET {eeee} -> 200, address not null, address.street="123 Main St"
$r = Invoke-ApiRequest -Method GET -Url "$base/api/nestedobjects/$idEeee"
Assert-StatusCode $r 200 "GET /api/nestedobjects/{eeee} returns 200"
Assert-NotNull $r.Data.address "GET /api/nestedobjects/{eeee} address is not null"
Assert-Value $r.Data.address.street "123 Main St" "GET /api/nestedobjects/{eeee} address.street is '123 Main St'"

# 3. GET {ffff} -> 200, address is null
$r = Invoke-ApiRequest -Method GET -Url "$base/api/nestedobjects/$idFfff"
Assert-StatusCode $r 200 "GET /api/nestedobjects/{ffff} returns 200"
Assert-Null $r.Data.address "GET /api/nestedobjects/{ffff} address is null"

# 4. GET missing -> 404
$r = Invoke-ApiRequest -Method GET -Url "$base/api/nestedobjects/$idMissing"
Assert-StatusCode $r 404 "GET /api/nestedobjects/{missing} returns 404"

# 5. PATCH {address={street="456 Oak Ave", city="Shelbyville"}} on {eeee} -> 200, street updated
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/nestedobjects/$idEeee" -Body @{
    address = @{ street = "456 Oak Ave"; city = "Shelbyville" }
}
Assert-StatusCode $r 200 "PATCH /api/nestedobjects/{eeee} address updated returns 200"
Assert-Value $r.Data.address.street "456 Oak Ave" "PATCH /api/nestedobjects/{eeee} address.street updated to '456 Oak Ave'"
Assert-Value $r.Data.address.city "Shelbyville" "PATCH /api/nestedobjects/{eeee} address.city updated to 'Shelbyville'"

# 6. PATCH {address=null} on {eeee} -> 200, address is null
$bodyJson = [PSCustomObject]@{ address = $null } | ConvertTo-Json
$raw = Invoke-WebRequest -Method PATCH -Uri "$base/api/nestedobjects/$idEeee" -ContentType 'application/json' -Body $bodyJson -SkipHttpErrorCheck
$r = [PSCustomObject]@{ StatusCode = [int]$raw.StatusCode; Data = ($raw.Content | ConvertFrom-Json); IsSuccess = $raw.StatusCode -lt 400 }
Assert-StatusCode $r 200 "PATCH /api/nestedobjects/{eeee} address=null returns 200"
Assert-Null $r.Data.address "PATCH /api/nestedobjects/{eeee} address cleared to null"

# 7. PATCH {} empty on {ffff} -> 200, address still null
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/nestedobjects/$idFfff" -Body @{}
Assert-StatusCode $r 200 "PATCH /api/nestedobjects/{ffff} empty body returns 200"
Assert-Null $r.Data.address "PATCH /api/nestedobjects/{ffff} empty body leaves address null"

# 8. PATCH on missing id -> 404
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/nestedobjects/$idMissing" -Body @{ address = @{ street = "Ghost St"; city = "Nowhere" } }
Assert-StatusCode $r 404 "PATCH /api/nestedobjects/{missing} returns 404"

$totalFailures += Complete-Suite

###############################################################################
# Suite 4: Collections
###############################################################################
Initialize-Suite "PerPropertyAttributeApi — Collections (/api/collections)"

# 1. GET all -> 200, at least 2 items
$r = Invoke-ApiRequest -Method GET -Url "$base/api/collections"
Assert-StatusCode $r 200 "GET /api/collections returns 200"
Assert-NotNull $r.Data "GET /api/collections returns data"
$itemCount = @($r.Data).Count
Assert-Value ($itemCount -ge 2) $true "GET /api/collections returns at least 2 items"

# 2. GET {1111} -> 200, tags is not null
$r = Invoke-ApiRequest -Method GET -Url "$base/api/collections/$id1111"
Assert-StatusCode $r 200 "GET /api/collections/{1111} returns 200"
Assert-NotNull $r.Data.tags "GET /api/collections/{1111} tags is not null"

# 3. GET {6666} -> 200, tags is null
$r = Invoke-ApiRequest -Method GET -Url "$base/api/collections/$id6666"
Assert-StatusCode $r 200 "GET /api/collections/{6666} returns 200"
Assert-Null $r.Data.tags "GET /api/collections/{6666} tags is null"

# 4. GET missing -> 404
$r = Invoke-ApiRequest -Method GET -Url "$base/api/collections/$idMissing"
Assert-StatusCode $r 404 "GET /api/collections/{missing} returns 404"

# 5. PATCH {tags=["api","rest"]} on {1111} -> 200, tags replaced
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/collections/$id1111" -Body @{ tags = @("api", "rest") }
Assert-StatusCode $r 200 "PATCH /api/collections/{1111} tags replaced returns 200"
Assert-NotNull $r.Data.tags "PATCH /api/collections/{1111} tags is not null after replace"
Assert-Value $r.Data.tags[0] "api" "PATCH /api/collections/{1111} tags[0] is 'api'"
Assert-Value $r.Data.tags[1] "rest" "PATCH /api/collections/{1111} tags[1] is 'rest'"

# 6. PATCH {tags=null} on {1111} -> 200, tags is null
$bodyJson = [PSCustomObject]@{ tags = $null } | ConvertTo-Json
$raw = Invoke-WebRequest -Method PATCH -Uri "$base/api/collections/$id1111" -ContentType 'application/json' -Body $bodyJson -SkipHttpErrorCheck
$r = [PSCustomObject]@{ StatusCode = [int]$raw.StatusCode; Data = ($raw.Content | ConvertFrom-Json); IsSuccess = $raw.StatusCode -lt 400 }
Assert-StatusCode $r 200 "PATCH /api/collections/{1111} tags=null returns 200"
Assert-Null $r.Data.tags "PATCH /api/collections/{1111} tags cleared to null"

# 7. PATCH {} empty on {6666} -> 200, tags still null
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/collections/$id6666" -Body @{}
Assert-StatusCode $r 200 "PATCH /api/collections/{6666} empty body returns 200"
Assert-Null $r.Data.tags "PATCH /api/collections/{6666} empty body leaves tags null"

# 8. PATCH on missing id -> 404
$r = Invoke-ApiRequest -Method PATCH -Url "$base/api/collections/$idMissing" -Body @{ tags = @("ghost") }
Assert-StatusCode $r 404 "PATCH /api/collections/{missing} returns 404"

$totalFailures += Complete-Suite

###############################################################################
# Summary
###############################################################################
Write-Host ""
Write-Host "====================================="
Write-Host "TOTAL FAILURES: $totalFailures"
Write-Host "====================================="

exit $totalFailures
