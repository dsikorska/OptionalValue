using System.Text.Json.Serialization;

namespace System.Text.Json.OptionalValue;

/// <summary>
/// Wrapper type that distinguishes between "property not provided" and "property explicitly set to null/value".
/// Primarily used in HTTP PATCH operations to determine which fields should be updated.
/// </summary>
/// <typeparam name="T">The type of the wrapped value.</typeparam>
/// <remarks>
/// <para>
/// When deserializing JSON, this type tracks whether a property was present in the payload:
/// <list type="bullet">
/// <item><description><c>IsSpecified = false</c>: Property was absent from JSON → Don't update this field</description></item>
/// <item><description><c>IsSpecified = true, Value = null</c>: Property was explicitly set to null → Clear this field</description></item>
/// <item><description><c>IsSpecified = true, Value = some value</c>: Property had a value → Update to this value</description></item>
/// </list>
/// </para>
/// <para>
/// <strong>Example usage in a PATCH request model:</strong>
/// <code>
/// public class PatchUserRequest
/// {
///     [JsonConverter(typeof(OptionalValueJsonConverter&lt;string&gt;))]
///     public OptionalValue&lt;string&gt; Name { get; set; } = new();
///
///     [JsonConverter(typeof(OptionalValueJsonConverter&lt;string&gt;))]
///     public OptionalValue&lt;string&gt; Email { get; set; } = new();
///
///     [JsonConverter(typeof(OptionalValueJsonConverter&lt;DateTime?&gt;))]
///     public OptionalValue&lt;DateTime?&gt; ExpiresOn { get; set; } = new();
/// }
///
/// // In your PATCH handler:
/// if (request.Name.IsSpecified)
///     user.Name = request.Name.Value; // Update or clear
///
/// if (request.Email.IsSpecified)
///     user.Email = request.Email.Value; // Update or clear
///
/// if (request.ExpiresOn.IsSpecified)
///     user.ExpiresOn = request.ExpiresOn.Value; // Update or clear
/// </code>
/// </para>
/// </remarks>
#pragma warning disable CA1716 // OptionalValue is the standard name for this pattern
#pragma warning disable CA2225 // Implicit operators are intentional for convenience
public class OptionalValue<T>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OptionalValue{T}"/> class with no value specified.
	/// Sets <see cref="IsSpecified"/> to <c>false</c>.
	/// </summary>
	public OptionalValue()
	{
		IsSpecified = false;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="OptionalValue{T}"/> class with the specified value.
	/// Sets <see cref="IsSpecified"/> to <c>true</c>.
	/// </summary>
	/// <param name="value">The value to wrap. Can be null.</param>
	public OptionalValue(T? value)
	{
		Value = value;
		IsSpecified = true;
	}

	/// <summary>
	/// Gets or sets the wrapped value.
	/// This value is only meaningful when <see cref="IsSpecified"/> is <c>true</c>.
	/// </summary>
	[JsonPropertyName("value")]
	public T? Value { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the value was explicitly provided.
	/// <list type="bullet">
	/// <item><description><c>true</c>: The property was present in the JSON payload (even if null)</description></item>
	/// <item><description><c>false</c>: The property was absent from the JSON payload</description></item>
	/// </list>
	/// </summary>
	[JsonIgnore]
	public bool IsSpecified { get; set; }

	/// <summary>
	/// Implicitly converts a value of type <typeparamref name="T"/> to an <see cref="OptionalValue{T}"/>.
	/// Sets <see cref="IsSpecified"/> to <c>true</c>.
	/// </summary>
	/// <param name="value">The value to wrap.</param>
	public static implicit operator OptionalValue<T>(T? value) => new(value);

	/// <summary>
	/// Implicitly converts an <see cref="OptionalValue{T}"/> to the underlying value of type <typeparamref name="T"/>.
	/// Returns the <see cref="Value"/> property.
	/// </summary>
	/// <param name="optional">The optional value to unwrap.</param>
#pragma warning disable CA1062 // Null check not needed in our usage
	public static implicit operator T?(OptionalValue<T> optional) => optional.Value;
#pragma warning restore CA1062
}
#pragma warning restore CA2225
#pragma warning restore CA1716
