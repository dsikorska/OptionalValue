using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;

namespace System.Text.Json.OptionalValue.Tests;

[TestFixture]
public class OptionalValueConverterFactoryTests
{
	private JsonSerializerOptions _options = null!;

	[SetUp]
	public void SetUp()
	{
		_options = new JsonSerializerOptions();
		_options.Converters.Add(new OptionalValueConverterFactory());
	}

	[Test]
	public void CanConvert_WithOptionalValueType_ShouldReturnTrue()
	{
		// Arrange
		var factory = new OptionalValueConverterFactory();
		var type = typeof(OptionalValue<string>);

		// Act
		var result = factory.CanConvert(type);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public void CanConvert_WithNonOptionalValueType_ShouldReturnFalse()
	{
		// Arrange
		var factory = new OptionalValueConverterFactory();
		var type = typeof(string);

		// Act
		var result = factory.CanConvert(type);

		// Assert
		result.Should().BeFalse();
	}

	[Test]
	public void CanConvert_WithNullableOptionalValue_ShouldReturnTrue()
	{
		// Arrange
		var factory = new OptionalValueConverterFactory();
		var type = typeof(OptionalValue<int?>);

		// Act
		var result = factory.CanConvert(type);

		// Assert
		result.Should().BeTrue();
	}

	[Test]
	public void Deserialize_WithFactoryRegistered_NoAttributeNeeded_ShouldWork()
	{
		// Arrange
		var json = """{"name": "John Doe", "email": null}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModelWithoutAttributes>(json, _options);

		// Assert
		result.Should().NotBeNull();
		result!.Name.IsSpecified.Should().BeTrue();
		result.Name.Value.Should().Be("John Doe");
		result.Email.IsSpecified.Should().BeTrue();
		result.Email.Value.Should().BeNull();
		result.Age.IsSpecified.Should().BeFalse("age was not in JSON");
	}

	[Test]
	public void Serialize_WithFactoryRegistered_NoAttributeNeeded_ShouldWork()
	{
		// Arrange
		var model = new TestModelWithoutAttributes
		{
			Name = new OptionalValue<string>("Jane Smith"),
			Email = new OptionalValue<string>(null),
			Age = new OptionalValue<int?>() // Not specified
		};

		// Act
		var json = JsonSerializer.Serialize(model, _options);

		// Assert
		json.Should().Contain("\"name\":\"Jane Smith\"");
		json.Should().Contain("\"email\":null");
		json.Should().Contain("\"age\":null");
	}

	[Test]
	public void RoundTrip_WithFactoryRegistered_ShouldPreserveValues()
	{
		// Arrange
		var original = new TestModelWithoutAttributes
		{
			Name = new OptionalValue<string>("Test User"),
			Email = new OptionalValue<string>("test@example.com"),
			Age = new OptionalValue<int?>(25)
		};

		// Act
		var json = JsonSerializer.Serialize(original, _options);
		var deserialized = JsonSerializer.Deserialize<TestModelWithoutAttributes>(json, _options);

		// Assert
		deserialized.Should().NotBeNull();
		deserialized!.Name.IsSpecified.Should().BeTrue();
		deserialized.Name.Value.Should().Be("Test User");
		deserialized.Email.IsSpecified.Should().BeTrue();
		deserialized.Email.Value.Should().Be("test@example.com");
		deserialized.Age.IsSpecified.Should().BeTrue();
		deserialized.Age.Value.Should().Be(25);
	}

	[Test]
	public void Deserialize_WithComplexType_ShouldWork()
	{
		// Arrange
		var json = """{"address": {"street": "123 Main St", "city": "Springfield"}}""";

		// Act
		var result = JsonSerializer.Deserialize<ModelWithComplexType>(json, _options);

		// Assert
		result.Should().NotBeNull();
		result!.Address.IsSpecified.Should().BeTrue();
		result.Address.Value.Should().NotBeNull();
		result.Address.Value!.Street.Should().Be("123 Main St");
		result.Address.Value.City.Should().Be("Springfield");
	}

	[Test]
	public void Deserialize_WithNestedOptionalValues_ShouldWork()
	{
		// Arrange
		var json = """{"user": {"name": "John", "email": null}}""";

		// Act
		var result = JsonSerializer.Deserialize<ModelWithNestedOptionalValues>(json, _options);

		// Assert
		result.Should().NotBeNull();
		result!.User.IsSpecified.Should().BeTrue();
		result.User.Value.Should().NotBeNull();
		result.User.Value!.Name.IsSpecified.Should().BeTrue();
		result.User.Value.Name.Value.Should().Be("John");
		result.User.Value.Email.IsSpecified.Should().BeTrue();
		result.User.Value.Email.Value.Should().BeNull();
	}

	[Test]
	public void CreateConverter_WithInvalidType_ShouldThrow()
	{
		// Arrange
		var factory = new OptionalValueConverterFactory();
		var invalidType = typeof(string);

		// Act & Assert
		// Note: CreateConverter should only be called for types where CanConvert returns true
		// But testing error handling is still valuable
		var act = () => factory.CreateConverter(invalidType, _options);

		// This will throw because string doesn't have generic arguments
		act.Should().Throw<Exception>();
	}

	// Test models without [JsonConverter] attributes
	private class TestModelWithoutAttributes
	{
		[JsonPropertyName("name")]
		public OptionalValue<string> Name { get; set; } = new();

		[JsonPropertyName("email")]
		public OptionalValue<string> Email { get; set; } = new();

		[JsonPropertyName("age")]
		public OptionalValue<int?> Age { get; set; } = new();
	}

	private class ModelWithComplexType
	{
		[JsonPropertyName("address")]
		public OptionalValue<Address> Address { get; set; } = new();
	}

	private class Address
	{
		[JsonPropertyName("street")]
		public string Street { get; set; } = string.Empty;

		[JsonPropertyName("city")]
		public string City { get; set; } = string.Empty;
	}

	private class ModelWithNestedOptionalValues
	{
		[JsonPropertyName("user")]
		public OptionalValue<UserInfo> User { get; set; } = new();
	}

	private class UserInfo
	{
		[JsonPropertyName("name")]
		public OptionalValue<string> Name { get; set; } = new();

		[JsonPropertyName("email")]
		public OptionalValue<string> Email { get; set; } = new();
	}
}
