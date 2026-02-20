namespace System.Text.Json.OptionalValue;

/// <summary>
/// Extension methods for <see cref="JsonSerializerOptions"/> to simplify <see cref="OptionalValue{T}"/> configuration.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
	/// <summary>
	/// Adds automatic <see cref="OptionalValue{T}"/> support to the <see cref="JsonSerializerOptions"/>.
	/// After calling this method, all <see cref="OptionalValue{T}"/> properties will serialize/deserialize correctly
	/// without requiring per-property <c>[JsonConverter]</c> attributes.
	/// </summary>
	/// <param name="options">The <see cref="JsonSerializerOptions"/> to configure.</param>
	/// <returns>The same <see cref="JsonSerializerOptions"/> instance for method chaining.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <c>null</c>.</exception>
	/// <remarks>
	/// <para>
	/// <strong>Usage example:</strong>
	/// <code>
	/// // In Program.cs or Startup.cs:
	/// builder.Services.ConfigureHttpJsonOptions(options =>
	/// {
	///     options.SerializerOptions.AddOptionalValueSupport();
	/// });
	///
	/// // Or for ASP.NET Core MVC:
	/// builder.Services.AddControllers()
	///     .AddJsonOptions(options =>
	///     {
	///         options.JsonSerializerOptions.AddOptionalValueSupport();
	///     });
	///
	/// // Or for standalone JsonSerializerOptions:
	/// var options = new JsonSerializerOptions();
	/// options.AddOptionalValueSupport();
	/// </code>
	/// </para>
	/// <para>
	/// This method registers the <see cref="OptionalValueConverterFactory"/> which automatically handles
	/// all <see cref="OptionalValue{T}"/> types. After calling this, your models can use <see cref="OptionalValue{T}"/>
	/// without any attributes:
	/// <code>
	/// public class PatchUserRequest
	/// {
	///     // No [JsonConverter] attribute needed!
	///     public OptionalValue&lt;string&gt; Name { get; set; } = new();
	///     public OptionalValue&lt;string&gt; Email { get; set; } = new();
	///     public OptionalValue&lt;DateTime?&gt; ExpiresOn { get; set; } = new();
	/// }
	/// </code>
	/// </para>
	/// </remarks>
	public static JsonSerializerOptions AddOptionalValueSupport(this JsonSerializerOptions options)
	{
		if (options == null)
		{
			throw new ArgumentNullException(nameof(options));
		}

		// Check if factory is already registered to avoid duplicates
		bool alreadyRegistered = false;
		foreach (var converter in options.Converters)
		{
			if (converter is OptionalValueConverterFactory)
			{
				alreadyRegistered = true;
				break;
			}
		}

		if (!alreadyRegistered)
		{
			options.Converters.Add(new OptionalValueConverterFactory());
		}

		return options;
	}
}
