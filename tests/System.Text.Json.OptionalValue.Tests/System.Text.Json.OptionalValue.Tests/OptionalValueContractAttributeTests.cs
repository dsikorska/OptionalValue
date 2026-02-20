using FluentAssertions;
using NUnit.Framework;
using System.Reflection;
using System.Text.Json.Serialization;

namespace System.Text.Json.OptionalValue.Tests;

[TestFixture]
public class OptionalValueContractAttributeTests
{
	[Test]
	public void Attribute_CanBeAppliedToClass()
	{
		// Arrange & Act
		var type = typeof(TestModelWithAttribute);
		var attribute = type.GetCustomAttribute<OptionalValueContractAttribute>();

		// Assert
		attribute.Should().NotBeNull();
	}

	[Test]
	public void Attribute_CanBeAppliedToStruct()
	{
		// Arrange & Act
		var type = typeof(TestStructWithAttribute);
		var attribute = type.GetCustomAttribute<OptionalValueContractAttribute>();

		// Assert
		attribute.Should().NotBeNull();
	}

	[Test]
	public void Attribute_IsInherited()
	{
		// Arrange & Act
		var baseType = typeof(BaseModelWithAttribute);
		var derivedType = typeof(DerivedModel);

		var baseAttribute = baseType.GetCustomAttribute<OptionalValueContractAttribute>();
		var derivedAttribute = derivedType.GetCustomAttribute<OptionalValueContractAttribute>(inherit: true);

		// Assert
		baseAttribute.Should().NotBeNull();
		derivedAttribute.Should().NotBeNull("attribute should be inherited");
	}

	[Test]
	public void Attribute_WithFactoryRegistered_ShouldEnableOptionalValueSerialization()
	{
		// Arrange
		var options = new JsonSerializerOptions();
		options.AddOptionalValueSupport();

		var model = new TestModelWithAttribute
		{
			Name = new OptionalValue<string>("John Doe"),
			Email = new OptionalValue<string>(null)
		};

		// Act
		var json = JsonSerializer.Serialize(model, options);
		var deserialized = JsonSerializer.Deserialize<TestModelWithAttribute>(json, options);

		// Assert
		deserialized.Should().NotBeNull();
		deserialized!.Name.IsSpecified.Should().BeTrue();
		deserialized.Name.Value.Should().Be("John Doe");
		deserialized.Email.IsSpecified.Should().BeTrue();
		deserialized.Email.Value.Should().BeNull();
	}

	[Test]
	public void Attribute_WithFactoryRegistered_ShouldEnableOptionalValueDeserialization()
	{
		// Arrange
		var options = new JsonSerializerOptions();
		options.AddOptionalValueSupport();

		var json = """{"name": "Jane Smith", "email": null}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModelWithAttribute>(json, options);

		// Assert
		result.Should().NotBeNull();
		result!.Name.IsSpecified.Should().BeTrue();
		result.Name.Value.Should().Be("Jane Smith");
		result.Email.IsSpecified.Should().BeTrue();
		result.Email.Value.Should().BeNull();
		result.Age.IsSpecified.Should().BeFalse("age was not in JSON");
	}

	[Test]
	public void Attribute_DocumentsIntentForDevelopers()
	{
		// Arrange & Act
		var type = typeof(TestModelWithAttribute);
		var hasAttribute = type.IsDefined(typeof(OptionalValueContractAttribute), inherit: false);

		// Assert
		hasAttribute.Should().BeTrue("attribute serves as documentation that this class uses OptionalValue pattern");
	}

	[Test]
	public void Attribute_AllowsOnlyOneInstance()
	{
		// Arrange
		var attributeType = typeof(OptionalValueContractAttribute);
		var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

		// Assert
		attributeUsage.Should().NotBeNull();
		attributeUsage!.AllowMultiple.Should().BeFalse("only one attribute per class is allowed");
	}

	[Test]
	public void Attribute_CanOnlyBeAppliedToClassOrStruct()
	{
		// Arrange
		var attributeType = typeof(OptionalValueContractAttribute);
		var attributeUsage = attributeType.GetCustomAttribute<AttributeUsageAttribute>();

		// Assert
		attributeUsage.Should().NotBeNull();
		attributeUsage!.ValidOn.Should().Be(AttributeTargets.Class | AttributeTargets.Struct);
	}

	// Test models
	[OptionalValueContract]
	private class TestModelWithAttribute
	{
		[JsonPropertyName("name")]
		public OptionalValue<string> Name { get; set; } = new();

		[JsonPropertyName("email")]
		public OptionalValue<string> Email { get; set; } = new();

		[JsonPropertyName("age")]
		public OptionalValue<int?> Age { get; set; } = new();
	}

	[OptionalValueContract]
	private struct TestStructWithAttribute
	{
		[JsonPropertyName("value")]
		public OptionalValue<string> Value { get; set; }
	}

	[OptionalValueContract]
	private class BaseModelWithAttribute
	{
		[JsonPropertyName("id")]
		public OptionalValue<int> Id { get; set; } = new();
	}

	private class DerivedModel : BaseModelWithAttribute
	{
		[JsonPropertyName("name")]
		public OptionalValue<string> Name { get; set; } = new();
	}
}
