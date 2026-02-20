using System.Reflection;
using System.Text.Json.Serialization;

namespace System.Text.Json.OptionalValue;

/// <summary>
/// JSON converter factory that creates <see cref="OptionalValueJsonConverter{T}"/> instances for any <see cref="OptionalValue{T}"/> type.
/// This factory enables automatic conversion of all <see cref="OptionalValue{T}"/> properties without requiring per-property <c>[JsonConverter]</c> attributes.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Usage:</strong> Register this factory once in your application startup:
/// <code>
/// // In Program.cs or Startup.cs:
/// builder.Services.ConfigureHttpJsonOptions(options =>
/// {
///     options.SerializerOptions.Converters.Add(new OptionalValueConverterFactory());
/// });
///
/// // Or use the extension method:
/// options.SerializerOptions.AddOptionalValueSupport();
/// </code>
/// </para>
/// <para>
/// After registration, all <see cref="OptionalValue{T}"/> properties in your models will automatically serialize/deserialize correctly:
/// <code>
/// public class PatchRequest
/// {
///     // No [JsonConverter] attribute needed!
///     public OptionalValue&lt;string&gt; Name { get; set; } = new();
///     public OptionalValue&lt;DateTime?&gt; ExpiresOn { get; set; } = new();
/// }
/// </code>
/// </para>
/// </remarks>
public class OptionalValueConverterFactory : JsonConverterFactory
{
	/// <summary>
	/// Determines whether this factory can convert the specified type.
	/// </summary>
	/// <param name="typeToConvert">The type to check.</param>
	/// <returns><c>true</c> if the type is <see cref="OptionalValue{T}"/>; otherwise, <c>false</c>.</returns>
	public override bool CanConvert(Type typeToConvert)
	{
		if (!typeToConvert.IsGenericType)
		{
			return false;
		}

		var genericTypeDefinition = typeToConvert.GetGenericTypeDefinition();
		return genericTypeDefinition == typeof(OptionalValue<>);
	}

	/// <summary>
	/// Creates a <see cref="OptionalValueJsonConverter{T}"/> instance for the specified <see cref="OptionalValue{T}"/> type.
	/// </summary>
	/// <param name="typeToConvert">The type to convert (must be <see cref="OptionalValue{T}"/>).</param>
	/// <param name="options">The serializer options.</param>
	/// <returns>A <see cref="JsonConverter"/> instance that can handle the specified type.</returns>
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		// Extract the T from OptionalValue<T>
		Type valueType = typeToConvert.GetGenericArguments()[0];

		// Create OptionalValueJsonConverter<T> using reflection
		Type converterType = typeof(OptionalValueJsonConverter<>).MakeGenericType(valueType);

		JsonConverter? converter = (JsonConverter?)Activator.CreateInstance(converterType);

		if (converter == null)
		{
			throw new InvalidOperationException($"Failed to create converter for type {typeToConvert.FullName}");
		}

		return converter;
	}
}
