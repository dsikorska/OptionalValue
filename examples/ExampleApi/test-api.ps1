# Test script for OptionalValue Example API
# Run this after starting the API with: dotnet run

$BaseUrl = "https://localhost:5001"
$UserId = "11111111-1111-1111-1111-111111111111"
$ProductId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "OptionalValue Example API - Test Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Get all users
Write-Host "Test 1: GET /api/users (Get all users)" -ForegroundColor Yellow
Invoke-RestMethod -Uri "$BaseUrl/api/users" -Method Get -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Read-Host "Press Enter to continue"

# Test 2: Get user by ID
Write-Host "Test 2: GET /api/users/$UserId (Get user by ID)" -ForegroundColor Yellow
Invoke-RestMethod -Uri "$BaseUrl/api/users/$UserId" -Method Get -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Read-Host "Press Enter to continue"

# Test 3: PATCH - Update only name
Write-Host "Test 3: PATCH /api/users/$UserId (Update only name)" -ForegroundColor Yellow
$body = @{ name = "John Updated" } | ConvertTo-Json
Invoke-RestMethod -Uri "$BaseUrl/api/users/$UserId" -Method Patch -Body $body -ContentType "application/json" -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Write-Host "✅ Notice: Only 'name' was updated, other fields remained unchanged" -ForegroundColor Green
Read-Host "Press Enter to continue"

# Test 4: PATCH - Clear bio (set to null)
Write-Host "Test 4: PATCH /api/users/$UserId (Clear bio)" -ForegroundColor Yellow
$body = @{ bio = $null } | ConvertTo-Json
Invoke-RestMethod -Uri "$BaseUrl/api/users/$UserId" -Method Patch -Body $body -ContentType "application/json" -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Write-Host "✅ Notice: 'bio' was explicitly cleared (set to null)" -ForegroundColor Green
Read-Host "Press Enter to continue"

# Test 5: PATCH - Update multiple fields
Write-Host "Test 5: PATCH /api/users/$UserId (Update multiple fields)" -ForegroundColor Yellow
$body = @{
    name = "Jane Smith"
    email = "jane.smith@example.com"
    bio = "Product Manager"
    expiresOn = "2026-12-31T23:59:59Z"
    isActive = $true
} | ConvertTo-Json
Invoke-RestMethod -Uri "$BaseUrl/api/users/$UserId" -Method Patch -Body $body -ContentType "application/json" -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Write-Host "✅ Notice: All provided fields were updated" -ForegroundColor Green
Read-Host "Press Enter to continue"

# Test 6: PATCH - Empty request (no updates)
Write-Host "Test 6: PATCH /api/users/$UserId (Empty request - no updates)" -ForegroundColor Yellow
$body = @{} | ConvertTo-Json
Invoke-RestMethod -Uri "$BaseUrl/api/users/$UserId" -Method Patch -Body $body -ContentType "application/json" -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Write-Host "✅ Notice: No fields were updated (all remained unchanged)" -ForegroundColor Green
Read-Host "Press Enter to continue"

# Test 7: Get all products
Write-Host "Test 7: GET /api/products (Get all products)" -ForegroundColor Yellow
Invoke-RestMethod -Uri "$BaseUrl/api/products" -Method Get -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Read-Host "Press Enter to continue"

# Test 8: PATCH - Update product price only
Write-Host "Test 8: PATCH /api/products/$ProductId (Update only price)" -ForegroundColor Yellow
$body = @{ price = 899.99 } | ConvertTo-Json
Invoke-RestMethod -Uri "$BaseUrl/api/products/$ProductId" -Method Patch -Body $body -ContentType "application/json" -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Write-Host "✅ Notice: Only 'price' was updated" -ForegroundColor Green
Read-Host "Press Enter to continue"

# Test 9: PATCH - Clear product description
Write-Host "Test 9: PATCH /api/products/$ProductId (Clear description)" -ForegroundColor Yellow
$body = @{ description = $null } | ConvertTo-Json
Invoke-RestMethod -Uri "$BaseUrl/api/products/$ProductId" -Method Patch -Body $body -ContentType "application/json" -SkipCertificateCheck | ConvertTo-Json -Depth 10
Write-Host ""
Write-Host "✅ Notice: 'description' was cleared" -ForegroundColor Green
Write-Host ""

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "All tests completed!" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
