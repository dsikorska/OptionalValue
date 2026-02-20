# OptionalValue Example API

This is a complete ASP.NET Core Web API example demonstrating the `System.Text.Json.OptionalValue` library for implementing HTTP PATCH operations.

## 🎯 What This Example Demonstrates

1. **Global Registration** - One-line setup in `Program.cs`
2. **Clean Models** - No `[JsonConverter]` attributes needed
3. **PATCH Operations** - Proper partial updates with field tracking
4. **Two Usage Patterns**:
   - **Users**: Global registration (no attributes)
   - **Products**: Per-class attribute (`[OptionalValueContract]`)

## 🚀 Quick Start

### 1. Run the API

```bash
cd examples/ExampleApi
dotnet run
```

The API will start on `https://localhost:5001` (or `http://localhost:5000`).

### 2. Open Swagger UI

Navigate to: **https://localhost:5001**

The Swagger UI provides interactive API documentation where you can test all endpoints.

## 📋 Available Endpoints

### Users API (Global Registration Pattern)

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `PATCH /api/users/{id}` - Partially update user
- `DELETE /api/users/{id}` - Delete user

**Sample User IDs:**
- `11111111-1111-1111-1111-111111111111`
- `22222222-2222-2222-2222-222222222222`

### Products API (Per-Class Attribute Pattern)

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `PATCH /api/products/{id}` - Partially update product
- `DELETE /api/products/{id}` - Delete product

**Sample Product IDs:**
- `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`
- `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`

## 💡 PATCH Examples

### Example 1: Update Only Name (Users)

**Request:**
```bash
curl -X PATCH https://localhost:5001/api/users/11111111-1111-1111-1111-111111111111 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Updated"
  }'
```

**Result:**
- ✅ `name` is updated to "John Updated"
- ⏭️ `email`, `bio`, `expiresOn`, `isActive` remain unchanged

---

### Example 2: Clear Bio (Set to Null)

**Request:**
```bash
curl -X PATCH https://localhost:5001/api/users/11111111-1111-1111-1111-111111111111 \
  -H "Content-Type: application/json" \
  -d '{
    "bio": null
  }'
```

**Result:**
- ✅ `bio` is cleared (set to null)
- ⏭️ All other fields remain unchanged

---

### Example 3: Update Multiple Fields

**Request:**
```bash
curl -X PATCH https://localhost:5001/api/users/11111111-1111-1111-1111-111111111111 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Jane Smith",
    "email": "jane.smith@example.com",
    "bio": "Product Manager",
    "expiresOn": "2026-12-31T23:59:59Z",
    "isActive": true
  }'
```

**Result:**
- ✅ All provided fields are updated
- ⏭️ No fields remain unchanged (all were provided)

---

### Example 4: Remove Expiration

**Request:**
```bash
curl -X PATCH https://localhost:5001/api/users/22222222-2222-2222-2222-222222222222 \
  -H "Content-Type: application/json" \
  -d '{
    "expiresOn": null
  }'
```

**Result:**
- ✅ `expiresOn` is cleared (account never expires)
- ⏭️ All other fields remain unchanged

---

### Example 5: Update Product Price Only

**Request:**
```bash
curl -X PATCH https://localhost:5001/api/products/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa \
  -H "Content-Type: application/json" \
  -d '{
    "price": 899.99
  }'
```

**Result:**
- ✅ `price` is updated to 899.99
- ⏭️ `name`, `description`, `stock`, `category` remain unchanged

---

## 🔍 How It Works

### 1. Setup in Program.cs

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ⭐ ONE-LINE SETUP
        options.JsonSerializerOptions.AddOptionalValueSupport();
    });
```

### 2. Clean Models (No Attributes!)

```csharp
public class PatchUserRequest
{
    // No [JsonConverter] attributes needed!
    public OptionalValue<string> Name { get; set; } = new();
    public OptionalValue<string> Email { get; set; } = new();
    public OptionalValue<DateTime?> ExpiresOn { get; set; } = new();
}
```

### 3. PATCH Handler

```csharp
[HttpPatch("{id}")]
public IActionResult Patch(Guid id, [FromBody] PatchUserRequest request)
{
    var user = _userService.GetById(id);

    // Only update fields that were provided
    if (request.Name.IsSpecified)
        user.Name = request.Name.Value;

    if (request.Email.IsSpecified)
        user.Email = request.Email.Value;

    if (request.ExpiresOn.IsSpecified)
        user.ExpiresOn = request.ExpiresOn.Value;

    _userService.Save(user);
    return Ok(user);
}
```

## 🎓 Key Learning Points

### ✅ What Gets Updated

```json
{
  "name": "New Name",
  "email": "new@example.com"
}
```
→ Only `name` and `email` are updated. Other fields remain unchanged.

### ✅ Setting Field to Null

```json
{
  "bio": null
}
```
→ `bio` is explicitly set to null (cleared). Not the same as field being absent.

### ✅ Field Absent from Request

```json
{
  "name": "New Name"
}
```
→ `email`, `bio`, `expiresOn`, `isActive` are NOT in the request, so they remain unchanged.

## 📁 Project Structure

```
ExampleApi/
├── Controllers/
│   ├── UsersController.cs         # PATCH /api/users/{id}
│   └── ProductsController.cs      # PATCH /api/products/{id}
├── Models/
│   ├── User.cs                    # User entity
│   ├── PatchUserRequest.cs        # ⭐ Global registration pattern
│   ├── Product.cs                 # Product entity
│   └── PatchProductRequest.cs     # ⭐ Per-class attribute pattern
├── Services/
│   ├── IUserService.cs
│   ├── UserService.cs             # In-memory user management
│   ├── IProductService.cs
│   └── ProductService.cs          # In-memory product management
├── Program.cs                      # ⭐ AddOptionalValueSupport() setup
└── README.md                       # This file
```

## 🧪 Testing with Swagger UI

1. **Navigate to** https://localhost:5001
2. **Expand** any PATCH endpoint (e.g., `PATCH /api/users/{id}`)
3. **Click** "Try it out"
4. **Enter** a sample ID (e.g., `11111111-1111-1111-1111-111111111111`)
5. **Modify** the JSON body (remove or change fields)
6. **Click** "Execute"
7. **Observe** the response - only specified fields are updated

## 🔄 Testing Scenarios

### Scenario 1: Update Single Field
```json
{ "name": "Updated Name" }
```
Expected: Only name changes

### Scenario 2: Clear Optional Field
```json
{ "bio": null }
```
Expected: Bio is cleared (set to null)

### Scenario 3: Empty Request
```json
{}
```
Expected: No fields are updated (all remain unchanged)

### Scenario 4: Update All Fields
```json
{
  "name": "New Name",
  "email": "new@example.com",
  "bio": "New bio",
  "expiresOn": "2027-12-31T23:59:59Z",
  "isActive": false
}
```
Expected: All fields are updated

## 🎯 Comparison: With vs Without OptionalValue

### ❌ Without OptionalValue (Traditional Approach)

```json
PATCH /api/users/123
{ "name": "New Name" }
```

**Problem:** Can't distinguish between:
- Field not provided → Don't update
- Field set to null → Clear it

**Traditional Solution:** Use separate flags or complex DTOs
```csharp
public class PatchUserRequest
{
    public string? Name { get; set; }
    public bool UpdateName { get; set; }  // Ugly!

    public string? Email { get; set; }
    public bool UpdateEmail { get; set; }  // More boilerplate!
}
```

### ✅ With OptionalValue (Clean Approach)

```csharp
public class PatchUserRequest
{
    public OptionalValue<string> Name { get; set; } = new();
    public OptionalValue<string> Email { get; set; } = new();
    // Clean, no boilerplate!
}
```

**Handler:**
```csharp
if (request.Name.IsSpecified)
    user.Name = request.Name.Value;  // Simple and clear
```

## 🚀 Next Steps

1. **Explore** the code in `Controllers/`, `Models/`, and `Services/`
2. **Try** different PATCH requests via Swagger UI
3. **Check** the logs to see which fields are marked as `IsSpecified`
4. **Read** the main library README for more advanced usage

## 📚 Additional Resources

- [System.Text.Json.OptionalValue on NuGet](https://www.nuget.org/packages/System.Text.Json.OptionalValue/)
- [Main Library README](../../README.md)
- [RFC 7386 - JSON Merge Patch](https://datatracker.ietf.org/doc/html/rfc7386)

## 💬 Feedback

This example demonstrates best practices for implementing PATCH operations in ASP.NET Core using `OptionalValue<T>`. If you have suggestions or find issues, please open an issue on GitHub!
