using System.Text.Json.Serialization;

namespace System.Text.Json.OptionalValue;

/// <summary>
/// JSON converter for <see cref="OptionalValue{T}"/> that properly tracks whether a property was present in the JSON payload.
/// Sets <see cref="OptionalValue{T}.IsSpecified"/> to <c>true</c> when the property exists in JSON (whether null or a value).
/// </summary>
/// <typeparam name="T">The type of the value wrapped by the <see cref="OptionalValue{T}"/>.</typeparam>
/// <remarks>
/// <para>
/// This converter enables the <see cref="OptionalValue{T}"/> pattern by handling JSON deserialization:
/// <list type="bullet">
/// <item><description>Property absent from JSON → <c>IsSpecified = false</c> (default constructor)</description></item>
/// <item><description>Property present as null → <c>IsSpecified = true, Value = null</c></description></item>
/// <item><description>Property present with value → <c>IsSpecified = true, Value = deserialized value</c></description></item>
/// </list>
/// </para>
/// <para>
/// <strong>Usage:</strong> Apply this converter using <c>[JsonConverter]</c> attribute on properties:
/// <code>
/// public class PatchRequest
/// {
///     [JsonConverter(typeof(OptionalValueJsonConverter&lt;string&gt;))]
///     public OptionalValue&lt;string&gt; Name { get; set; } = new();
///
///     [JsonConverter(typeof(OptionalValueJsonConverter&lt;DateTime?&gt;))]
///     public OptionalValue&lt;DateTime?&gt; ExpiresOn { get; set; } = new();
/// }
/// </code>
/// </para>
/// <para>
/// <strong>Serialization limitation:</strong> When serializing, <c>IsSpecified = false</c> writes as <c>null</c>,
/// which deserializes back as <c>IsSpecified = true, Value = null</c>. This means you cannot distinguish
/// "field not specified" from "field explicitly set to null" after serialization round-trip.
/// </para>
/// </remarks>
public class OptionalValueJsonConverter<T> : JsonConverter<OptionalValue<T>>
{
	/// <summary>
	/// Gets a value indicating whether null values should be handled by this converter.
	/// Returns <c>true</c> to allow proper tracking of explicit null values in JSON.
	/// </summary>
	public override bool HandleNull => true;

	/// <summary>
	/// Reads and converts the JSON to an <see cref="OptionalValue{T}"/>.
	/// If this method is called, the property was present in the JSON payload.
	/// </summary>
	/// <param name="reader">The reader to read JSON from.</param>
	/// <param name="typeToConvert">The type to convert.</param>
	/// <param name="options">Serializer options.</param>
	/// <returns>An <see cref="OptionalValue{T}"/> with <see cref="OptionalValue{T}.IsSpecified"/> set to <c>true</c>.</returns>
	public override OptionalValue<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		// If we reach here, the property was present in the JSON payload
		// Handle explicit null in JSON (e.g., "expiresOn": null)
		if (reader.TokenType == JsonTokenType.Null)
		{
			return new OptionalValue<T>(default) { IsSpecified = true };
		}

		var value = JsonSerializer.Deserialize<T>(ref reader, options);
		return new OptionalValue<T>(value) { IsSpecified = true };
	}

	/// <summary>
	/// Writes an <see cref="OptionalValue{T}"/> to JSON.
	/// </summary>
	/// <param name="writer">The writer to write JSON to.</param>
	/// <param name="value">The <see cref="OptionalValue{T}"/> to serialize.</param>
	/// <param name="options">Serializer options.</param>
	/// <remarks>
	/// <para>
	/// When <see cref="OptionalValue{T}.IsSpecified"/> is <c>false</c>, writes <c>null</c> (field will be omitted if property has
	/// <c>[JsonIgnore(Condition = WhenWritingNull)]</c>).
	/// </para>
	/// <para>
	/// When <see cref="OptionalValue{T}.IsSpecified"/> is <c>true</c>, writes the actual <see cref="OptionalValue{T}.Value"/> (even if null).
	/// </para>
	/// <para>
	/// <strong>Limitation:</strong> <c>IsSpecified = false</c> serializes as <c>null</c>, which deserializes as
	/// <c>IsSpecified = true, Value = null</c>. You cannot distinguish "field not specified" from
	/// "field explicitly set to null" after serialization.
	/// </para>
	/// </remarks>
	public override void Write(Utf8JsonWriter writer, OptionalValue<T> value, JsonSerializerOptions options)
	{
#pragma warning disable CA1062 // JSON serializer framework ensures non-null
		// When IsSpecified=false, write null (field will be omitted if property has JsonIgnore(Condition=WhenWritingNull))
		// When IsSpecified=true, write the actual value (even if null)
		// Note: IsSpecified=false serializes as null, which deserializes as IsSpecified=true with Value=null
		// This is a limitation - you cannot distinguish "field not specified" from "field explicitly set to null" after serialization
		if (!value.IsSpecified)
		{
			writer.WriteNullValue();
			return;
		}

		JsonSerializer.Serialize(writer, value.Value, options);
#pragma warning restore CA1062
	}
}
