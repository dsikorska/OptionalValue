using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;

namespace System.Text.Json.OptionalValue.Tests;

[TestFixture]
public class JsonSerializerOptionsExtensionsTests
{
	[Test]
	public void AddOptionalValueSupport_WithValidOptions_ShouldAddFactory()
	{
		// Arrange
		var options = new JsonSerializerOptions();

		// Act
		options.AddOptionalValueSupport();

		// Assert
		options.Converters.Should().ContainSingle(c => c is OptionalValueConverterFactory);
	}

	[Test]
	public void AddOptionalValueSupport_CalledMultipleTimes_ShouldNotAddDuplicates()
	{
		// Arrange
		var options = new JsonSerializerOptions();

		// Act
		options.AddOptionalValueSupport();
		options.AddOptionalValueSupport();
		options.AddOptionalValueSupport();

		// Assert
		var factories = options.Converters.Where(c => c is OptionalValueConverterFactory).ToList();
		factories.Should().HaveCount(1, "factory should only be added once even with multiple calls");
	}

	[Test]
	public void AddOptionalValueSupport_ShouldReturnSameInstance()
	{
		// Arrange
		var options = new JsonSerializerOptions();

		// Act
		var result = options.AddOptionalValueSupport();

		// Assert
		result.Should().BeSameAs(options, "extension method should return same instance for fluent chaining");
	}

	[Test]
	public void AddOptionalValueSupport_WithNull_ShouldThrowArgumentNullException()
	{
		// Arrange
		JsonSerializerOptions? options = null;

		// Act
		var act = () => options!.AddOptionalValueSupport();

		// Assert
		act.Should().Throw<ArgumentNullException>()
			.WithParameterName("options");
	}

	[Test]
	public void AddOptionalValueSupport_ShouldEnableOptionalValueSerialization()
	{
		// Arrange
		var options = new JsonSerializerOptions();
		options.AddOptionalValueSupport();

		var model = new TestModel
		{
			Name = new OptionalValue<string>("John Doe"),
			Email = new OptionalValue<string>(null),
			Age = new OptionalValue<int?>() // Not specified
		};

		// Act
		var json = JsonSerializer.Serialize(model, options);
		var deserialized = JsonSerializer.Deserialize<TestModel>(json, options);

		// Assert
		deserialized.Should().NotBeNull();
		deserialized!.Name.IsSpecified.Should().BeTrue();
		deserialized.Name.Value.Should().Be("John Doe");
		deserialized.Email.IsSpecified.Should().BeTrue();
		deserialized.Email.Value.Should().BeNull();
		deserialized.Age.IsSpecified.Should().BeTrue("null was in JSON");
		deserialized.Age.Value.Should().BeNull();
	}

	[Test]
	public void AddOptionalValueSupport_ShouldEnableOptionalValueDeserialization()
	{
		// Arrange
		var options = new JsonSerializerOptions();
		options.AddOptionalValueSupport();

		var json = """{"name": "Jane Smith", "email": null}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json, options);

		// Assert
		result.Should().NotBeNull();
		result!.Name.IsSpecified.Should().BeTrue();
		result.Name.Value.Should().Be("Jane Smith");
		result.Email.IsSpecified.Should().BeTrue();
		result.Email.Value.Should().BeNull();
		result.Age.IsSpecified.Should().BeFalse("age was not in JSON");
	}

	[Test]
	public void AddOptionalValueSupport_WithFluentChaining_ShouldWork()
	{
		// Arrange & Act
		var options = new JsonSerializerOptions()
			.AddOptionalValueSupport();

		options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.WriteIndented = true;

		var model = new TestModel
		{
			Name = new OptionalValue<string>("Test"),
			Email = new OptionalValue<string>("test@example.com"),
			Age = new OptionalValue<int?>(30)
		};

		var json = JsonSerializer.Serialize(model, options);

		// Assert
		json.Should().Contain("\"name\"");
		json.Should().Contain("\"email\"");
		json.Should().Contain("\"age\"");
		json.Should().Contain("Test");
		json.Should().Contain("test@example.com");
		json.Should().Contain("30");
	}

	[Test]
	public void AddOptionalValueSupport_WithExistingConverters_ShouldPreserveThem()
	{
		// Arrange
		var options = new JsonSerializerOptions();
		var customConverter = new CustomStringConverter();
		options.Converters.Add(customConverter);

		// Act
		options.AddOptionalValueSupport();

		// Assert
		options.Converters.Should().HaveCount(2);
		options.Converters.Should().Contain(customConverter);
		options.Converters.Should().ContainSingle(c => c is OptionalValueConverterFactory);
	}

	// Test model
	private class TestModel
	{
		[JsonPropertyName("name")]
		public OptionalValue<string> Name { get; set; } = new();

		[JsonPropertyName("email")]
		public OptionalValue<string> Email { get; set; } = new();

		[JsonPropertyName("age")]
		public OptionalValue<int?> Age { get; set; } = new();
	}

	// Custom converter for testing
	private class CustomStringConverter : JsonConverter<string>
	{
		public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return reader.GetString();
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value);
		}
	}
}
