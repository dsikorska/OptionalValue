namespace System.Text.Json.OptionalValue;

/// <summary>
/// Marks a class to automatically apply <see cref="OptionalValue{T}"/> JSON serialization behavior
/// to all <see cref="OptionalValue{T}"/> properties without requiring per-property <c>[JsonConverter]</c> attributes.
/// </summary>
/// <remarks>
/// <para>
/// This attribute provides an alternative to global registration via <see cref="JsonSerializerOptionsExtensions.AddOptionalValueSupport"/>.
/// It allows explicit opt-in at the class level rather than globally.
/// </para>
/// <para>
/// <strong>Usage:</strong>
/// <code>
/// [OptionalValueContract]
/// public class PatchUserRequest
/// {
///     // No [JsonConverter] attributes needed on properties
///     public OptionalValue&lt;string&gt; Name { get; set; } = new();
///     public OptionalValue&lt;string&gt; Email { get; set; } = new();
///     public OptionalValue&lt;DateTime?&gt; ExpiresOn { get; set; } = new();
/// }
/// </code>
/// </para>
/// <para>
/// <strong>Note:</strong> When using this attribute, you must still register the <see cref="OptionalValueConverterFactory"/>
/// in your <see cref="JsonSerializerOptions"/>:
/// <code>
/// options.SerializerOptions.AddOptionalValueSupport();
/// </code>
/// The attribute serves as documentation and future extensibility (e.g., for source generators or validation).
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public sealed class OptionalValueContractAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OptionalValueContractAttribute"/> class.
	/// </summary>
	public OptionalValueContractAttribute()
	{
	}
}
