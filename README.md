# System.Text.Json.OptionalValue

[![NuGet](https://img.shields.io/nuget/v/System.Text.Json.OptionalValue.svg)](https://www.nuget.org/packages/System.Text.Json.OptionalValue/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A lightweight .NET library providing `OptionalValue<T>` wrapper type for distinguishing between "property not provided" and "property explicitly set to null/value" in JSON PATCH operations with `System.Text.Json`.

## The Problem

When implementing HTTP PATCH endpoints, you need to distinguish between three states:
1. **Field not provided** in request → Don't update this field
2. **Field explicitly set to null** → Clear/remove this field's value
3. **Field set to a value** → Update to this new value

Standard C# nullable types (`string?`, `int?`) can't distinguish between cases #1 and #2 because both deserialize as `null`.

## The Solution

`OptionalValue<T>` wraps your value and tracks whether it was present in the JSON payload:

```csharp
public class OptionalValue<T>
{
    public T? Value { get; set; }           // The actual value
    public bool IsSpecified { get; set; }   // Was it in the JSON?
}
```

## Installation

```bash
dotnet add package System.Text.Json.OptionalValue
```

## Quick Start (Recommended)

### 1. **One-Time Setup** - Register in `Program.cs`

```csharp
using System.Text.Json.OptionalValue;

var builder = WebApplication.CreateBuilder(args);

// Option A: For minimal APIs and controllers
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.AddOptionalValueSupport();
});

// Option B: For MVC/API controllers only
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.AddOptionalValueSupport();
    });
```

### 2. **Define Your PATCH Request Model** - No Attributes Needed!

```csharp
using System.Text.Json.OptionalValue;

public class PatchUserRequest
{
    // Clean! No [JsonConverter] attributes needed
    public OptionalValue<string> Name { get; set; } = new();
    public OptionalValue<string> Email { get; set; } = new();
    public OptionalValue<DateTime?> ExpiresOn { get; set; } = new();
}
```

### 3. **Handle the PATCH Request**

```csharp
[HttpPatch("users/{id}")]
public IActionResult PatchUser(Guid id, [FromBody] PatchUserRequest request)
{
    var user = _repository.GetUser(id);

    // Only update fields that were provided in the request
    if (request.Name.IsSpecified)
        user.Name = request.Name.Value;  // Update or clear

    if (request.Email.IsSpecified)
        user.Email = request.Email.Value;  // Update or clear

    if (request.ExpiresOn.IsSpecified)
        user.ExpiresOn = request.ExpiresOn.Value;  // Update or clear

    _repository.SaveChanges();
    return Ok(user);
}
```

## Usage Options

This library provides **three ways** to use `OptionalValue<T>` - choose based on your preference:

### ✅ Option 1: Global Registration (Recommended)

**Best for**: Most applications - set it once and forget it.

```csharp
// Program.cs - ONE TIME SETUP
options.SerializerOptions.AddOptionalValueSupport();

// Models - CLEAN, no attributes
public class PatchRequest
{
    public OptionalValue<string> Name { get; set; } = new();
    public OptionalValue<string> Email { get; set; } = new();
}
```

**Pros**: Cleanest code, no attributes needed, works everywhere
**Cons**: Applies globally to all models

---

### ✅ Option 2: Per-Class Attribute

**Best for**: Explicit opt-in per model, better documentation.

```csharp
// Program.cs - still need one-time setup
options.SerializerOptions.AddOptionalValueSupport();

// Models - ONE attribute per class
[OptionalValueContract]
public class PatchUserRequest
{
    public OptionalValue<string> Name { get; set; } = new();
    public OptionalValue<string> Email { get; set; } = new();
}
```

**Pros**: Explicit intent, documents which models use the pattern
**Cons**: Requires attribute on each model class

---

### ✅ Option 3: Per-Property Attribute (Backward Compatible)

**Best for**: Fine-grained control, mixing with other converters.

```csharp
// No setup needed in Program.cs

// Models - attribute on EACH property
public class PatchUserRequest
{
    [JsonConverter(typeof(OptionalValueJsonConverter<string>))]
    public OptionalValue<string> Name { get; set; } = new();

    [JsonConverter(typeof(OptionalValueJsonConverter<string>))]
    public OptionalValue<string> Email { get; set; } = new();
}
```

**Pros**: Maximum control, no global registration needed
**Cons**: Most verbose, easy to forget attributes

---

## Example JSON Payloads

**Scenario 1: Update name only** (leave email and expiresOn unchanged)
```json
{
  "name": "John Doe"
}
```
Result:
- `Name.IsSpecified = true, Name.Value = "John Doe"` → Update
- `Email.IsSpecified = false` → Don't update
- `ExpiresOn.IsSpecified = false` → Don't update

**Scenario 2: Clear expiresOn** (set to null explicitly)
```json
{
  "expiresOn": null
}
```
Result:
- `Name.IsSpecified = false` → Don't update
- `Email.IsSpecified = false` → Don't update
- `ExpiresOn.IsSpecified = true, ExpiresOn.Value = null` → Clear

**Scenario 3: Update multiple fields**
```json
{
  "name": "Jane Smith",
  "email": "jane@example.com",
  "expiresOn": "2025-12-31T23:59:59Z"
}
```
Result: All three fields have `IsSpecified = true` → Update all

## Features

- ✅ **Lightweight**: Zero dependencies except `System.Text.Json`
- ✅ **Type-safe**: Generic `OptionalValue<T>` works with any type
- ✅ **Well-tested**: 50+ unit tests covering all scenarios
- ✅ **Implicit conversions**: Supports implicit conversion to/from underlying type
- ✅ **Nullable support**: Works seamlessly with nullable types (`OptionalValue<DateTime?>`)
- ✅ **Easy to use**: Three usage patterns - pick what fits your style
- ✅ **.NET Standard 2.1**: Compatible with .NET Core 3.0+, .NET 5+, .NET 6+, .NET 8+

## Advanced Usage

### With Record Types

```csharp
[OptionalValueContract]
public record PatchProductRequest
{
    public OptionalValue<string> Name { get; init; } = new();
    public OptionalValue<decimal?> Price { get; init; } = new();
}
```

### With Complex Types

```csharp
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

[OptionalValueContract]
public class PatchUserRequest
{
    public OptionalValue<Address> Address { get; set; } = new();
}
```

### With Nested OptionalValues

```csharp
public class UserInfo
{
    public OptionalValue<string> Name { get; set; } = new();
    public OptionalValue<string> Email { get; set; } = new();
}

[OptionalValueContract]
public class PatchRequest
{
    public OptionalValue<UserInfo> User { get; set; } = new();
}
```

### Standalone JsonSerializerOptions

```csharp
var options = new JsonSerializerOptions();
options.AddOptionalValueSupport();

var json = JsonSerializer.Serialize(myObject, options);
var obj = JsonSerializer.Deserialize<MyType>(json, options);
```

## API Reference

### `OptionalValue<T>`

```csharp
public class OptionalValue<T>
{
    // Constructors
    public OptionalValue();                 // IsSpecified = false
    public OptionalValue(T? value);         // IsSpecified = true

    // Properties
    public T? Value { get; set; }           // The wrapped value
    public bool IsSpecified { get; set; }   // Was it present in JSON?

    // Implicit conversions
    public static implicit operator OptionalValue<T>(T? value);
    public static implicit operator T?(OptionalValue<T> optional);
}
```

### `OptionalValueJsonConverter<T>`

```csharp
public class OptionalValueJsonConverter<T> : JsonConverter<OptionalValue<T>>
{
    public override bool HandleNull => true;
    // Handles JSON serialization/deserialization for a specific OptionalValue<T>
}
```

### `OptionalValueConverterFactory`

```csharp
public class OptionalValueConverterFactory : JsonConverterFactory
{
    // Automatically creates converters for any OptionalValue<T> type
    // Used internally by AddOptionalValueSupport()
}
```

### `JsonSerializerOptionsExtensions`

```csharp
public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions AddOptionalValueSupport(this JsonSerializerOptions options);
    // Registers OptionalValueConverterFactory for automatic OptionalValue<T> handling
}
```

### `OptionalValueContractAttribute`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public sealed class OptionalValueContractAttribute : Attribute
{
    // Marks a class/struct as using OptionalValue pattern
    // Serves as documentation and explicit opt-in
}
```

## Limitations

### Serialization Round-Trip

When serializing `OptionalValue<T>` with `IsSpecified = false`, it writes `null` to JSON. Upon deserialization, this becomes `IsSpecified = true, Value = null`. This means:

- ✅ **Deserialization (request handling)**: Works perfectly - you can distinguish absent fields
- ⚠️ **Serialization round-trip**: `IsSpecified = false` cannot be preserved after serialization

This is rarely an issue since the pattern is designed for **incoming** PATCH requests, not outgoing responses.

## Migration Guide

### From v1.0.0 (per-property attributes)

**Old code:**
```csharp
public class PatchRequest
{
    [JsonConverter(typeof(OptionalValueJsonConverter<string>))]
    public OptionalValue<string> Name { get; set; } = new();
}
```

**New code (recommended):**
```csharp
// Program.cs - add once
options.SerializerOptions.AddOptionalValueSupport();

// Model - remove attributes
public class PatchRequest
{
    public OptionalValue<string> Name { get; set; } = new();
}
```

Your old code will still work! The per-property attributes are still supported for backward compatibility.

## Performance

- **Factory pattern**: Creates converters once and caches them
- **No reflection overhead**: After initial converter creation
- **Minimal allocations**: OptionalValue<T> is a simple class wrapper

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

This pattern is inspired by:
- [JSON Merge Patch (RFC 7386)](https://datatracker.ietf.org/doc/html/rfc7386)
- [ASP.NET Core PATCH operations](https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch)
- Functional programming's `Optional<T>` / `Maybe<T>` types

## Support

- 📦 **NuGet**: [System.Text.Json.OptionalValue](https://www.nuget.org/packages/System.Text.Json.OptionalValue/)
- 🐛 **Issues**: [GitHub Issues](https://github.com/relativityone/System.Text.Json.OptionalValue/issues)
- 💬 **Discussions**: [GitHub Discussions](https://github.com/relativityone/System.Text.Json.OptionalValue/discussions)
