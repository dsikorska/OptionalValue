#!/bin/bash
# Test script for OptionalValue Example API
# Run this after starting the API with: dotnet run

BASE_URL="https://localhost:5001"
USER_ID="11111111-1111-1111-1111-111111111111"
PRODUCT_ID="aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"

echo "========================================="
echo "OptionalValue Example API - Test Script"
echo "========================================="
echo ""

# Test 1: Get all users
echo "Test 1: GET /api/users (Get all users)"
curl -k -s -X GET "$BASE_URL/api/users" | jq '.'
echo ""
echo "Press Enter to continue..."
read

# Test 2: Get user by ID
echo "Test 2: GET /api/users/$USER_ID (Get user by ID)"
curl -k -s -X GET "$BASE_URL/api/users/$USER_ID" | jq '.'
echo ""
echo "Press Enter to continue..."
read

# Test 3: PATCH - Update only name
echo "Test 3: PATCH /api/users/$USER_ID (Update only name)"
curl -k -s -X PATCH "$BASE_URL/api/users/$USER_ID" \
  -H "Content-Type: application/json" \
  -d '{"name": "John Updated"}' | jq '.'
echo ""
echo "✅ Notice: Only 'name' was updated, other fields remained unchanged"
echo "Press Enter to continue..."
read

# Test 4: PATCH - Clear bio (set to null)
echo "Test 4: PATCH /api/users/$USER_ID (Clear bio)"
curl -k -s -X PATCH "$BASE_URL/api/users/$USER_ID" \
  -H "Content-Type: application/json" \
  -d '{"bio": null}' | jq '.'
echo ""
echo "✅ Notice: 'bio' was explicitly cleared (set to null)"
echo "Press Enter to continue..."
read

# Test 5: PATCH - Update multiple fields
echo "Test 5: PATCH /api/users/$USER_ID (Update multiple fields)"
curl -k -s -X PATCH "$BASE_URL/api/users/$USER_ID" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Jane Smith",
    "email": "jane.smith@example.com",
    "bio": "Product Manager",
    "expiresOn": "2026-12-31T23:59:59Z",
    "isActive": true
  }' | jq '.'
echo ""
echo "✅ Notice: All provided fields were updated"
echo "Press Enter to continue..."
read

# Test 6: PATCH - Empty request (no updates)
echo "Test 6: PATCH /api/users/$USER_ID (Empty request - no updates)"
curl -k -s -X PATCH "$BASE_URL/api/users/$USER_ID" \
  -H "Content-Type: application/json" \
  -d '{}' | jq '.'
echo ""
echo "✅ Notice: No fields were updated (all remained unchanged)"
echo "Press Enter to continue..."
read

# Test 7: Get all products
echo "Test 7: GET /api/products (Get all products)"
curl -k -s -X GET "$BASE_URL/api/products" | jq '.'
echo ""
echo "Press Enter to continue..."
read

# Test 8: PATCH - Update product price only
echo "Test 8: PATCH /api/products/$PRODUCT_ID (Update only price)"
curl -k -s -X PATCH "$BASE_URL/api/products/$PRODUCT_ID" \
  -H "Content-Type: application/json" \
  -d '{"price": 899.99}' | jq '.'
echo ""
echo "✅ Notice: Only 'price' was updated"
echo "Press Enter to continue..."
read

# Test 9: PATCH - Clear product description
echo "Test 9: PATCH /api/products/$PRODUCT_ID (Clear description)"
curl -k -s -X PATCH "$BASE_URL/api/products/$PRODUCT_ID" \
  -H "Content-Type: application/json" \
  -d '{"description": null}' | jq '.'
echo ""
echo "✅ Notice: 'description' was cleared"
echo ""

echo "========================================="
echo "All tests completed!"
echo "========================================="
