using FluentAssertions;
using NUnit.Framework;
using System.Text.Json.Serialization;

namespace System.Text.Json.OptionalValue.Tests;

[TestFixture]
public class OptionalValueJsonConverterTests
{
	[Test]
	public void Deserialize_WithFieldAbsent_ShouldHaveIsSpecifiedFalse()
	{
		// Arrange
		var json = "{}";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalString.IsSpecified.Should().BeFalse();
		result.OptionalInt.IsSpecified.Should().BeFalse();
		result.OptionalDateTime.IsSpecified.Should().BeFalse();
	}

	[Test]
	public void Deserialize_WithFieldSetToNull_ShouldHaveIsSpecifiedTrueAndValueNull()
	{
		// Arrange
		var json = """{"optionalString": null, "optionalInt": null, "optionalDateTime": null}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalString.IsSpecified.Should().BeTrue();
		result.OptionalString.Value.Should().BeNull();
		result.OptionalInt.IsSpecified.Should().BeTrue();
		result.OptionalInt.Value.Should().BeNull();
		result.OptionalDateTime.IsSpecified.Should().BeTrue();
		result.OptionalDateTime.Value.Should().BeNull();
	}

	[Test]
	public void Deserialize_WithFieldSetToValue_ShouldHaveIsSpecifiedTrueAndCorrectValue()
	{
		// Arrange
		var expectedDateTime = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);
		var json = $$"""{"optionalString": "test-value", "optionalInt": 42, "optionalDateTime": "{{expectedDateTime:O}}"}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalString.IsSpecified.Should().BeTrue();
		result.OptionalString.Value.Should().Be("test-value");
		result.OptionalInt.IsSpecified.Should().BeTrue();
		result.OptionalInt.Value.Should().Be(42);
		result.OptionalDateTime.IsSpecified.Should().BeTrue();
		result.OptionalDateTime.Value.Should().Be(expectedDateTime);
	}

	[Test]
	public void Deserialize_WithMixedFields_ShouldHandleEachFieldIndependently()
	{
		// Arrange
		var json = """{"optionalString": "test", "optionalDateTime": null}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalString.IsSpecified.Should().BeTrue();
		result.OptionalString.Value.Should().Be("test");
		result.OptionalInt.IsSpecified.Should().BeFalse("optionalInt was not present in JSON");
		result.OptionalInt.Value.Should().BeNull();
		result.OptionalDateTime.IsSpecified.Should().BeTrue();
		result.OptionalDateTime.Value.Should().BeNull();
	}

	[Test]
	public void Serialize_WithIsSpecifiedFalse_ShouldWriteNullValue()
	{
		// Arrange
		var model = new TestModel
		{
			OptionalString = new OptionalValue<string>(),
			OptionalInt = new OptionalValue<int?>(),
			OptionalDateTime = new OptionalValue<DateTime?>()
		};

		// Act
		var json = JsonSerializer.Serialize(model);

		// Assert
		// When IsSpecified=false, the converter writes null to allow proper JSON serialization
		json.Should().Contain("\"optionalString\":null");
		json.Should().Contain("\"optionalInt\":null");
		json.Should().Contain("\"optionalDateTime\":null");
	}

	[Test]
	public void Serialize_WithIsSpecifiedTrueAndNullValue_ShouldIncludeFieldAsNull()
	{
		// Arrange
		var model = new TestModel
		{
			OptionalString = new OptionalValue<string>(null),
			OptionalInt = new OptionalValue<int?>(null),
			OptionalDateTime = new OptionalValue<DateTime?>(null)
		};

		// Act
		var json = JsonSerializer.Serialize(model);

		// Assert
		json.Should().Contain("\"optionalString\":null");
		json.Should().Contain("\"optionalInt\":null");
		json.Should().Contain("\"optionalDateTime\":null");
	}

	[Test]
	public void Serialize_WithIsSpecifiedTrueAndValue_ShouldIncludeFieldWithValue()
	{
		// Arrange
		var dateTime = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);
		var model = new TestModel
		{
			OptionalString = new OptionalValue<string>("test-value"),
			OptionalInt = new OptionalValue<int?>(42),
			OptionalDateTime = new OptionalValue<DateTime?>(dateTime)
		};

		// Act
		var json = JsonSerializer.Serialize(model);

		// Assert
		json.Should().MatchRegex("\"optionalString\"\\s*:\\s*\"test-value\"");
		json.Should().MatchRegex("\"optionalInt\"\\s*:\\s*42");
		json.Should().MatchRegex("\"optionalDateTime\"\\s*:\\s*\"2025-12-31T23:59:59(\\.0+)?Z\"");
	}

	[Test]
	public void HandleNull_Property_ShouldReturnTrue()
	{
		// Arrange
		var converter = new OptionalValueJsonConverter<string>();

		// Act & Assert
		converter.HandleNull.Should().BeTrue("HandleNull must be true to process null JSON values");
	}

	[Test]
	public void RoundTrip_WithNullValue_ShouldPreserveIsSpecifiedTrue()
	{
		// Arrange
		var original = new TestModel
		{
			OptionalString = new OptionalValue<string>(null)
		};

		// Act
		var json = JsonSerializer.Serialize(original);
		var deserialized = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		deserialized.Should().NotBeNull();
		deserialized!.OptionalString.IsSpecified.Should().BeTrue();
		deserialized.OptionalString.Value.Should().BeNull();
	}

	[Test]
	public void RoundTrip_WithAbsentField_CannotFullyPreserveIsSpecifiedFalse()
	{
		// Arrange
		var original = new TestModel
		{
			OptionalString = new OptionalValue<string>() // IsSpecified = false
		};

		// Act
		var json = JsonSerializer.Serialize(original);
		var deserialized = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		deserialized.Should().NotBeNull();
		deserialized!.OptionalString.IsSpecified.Should().BeTrue("null was present in JSON");
		deserialized.OptionalString.Value.Should().BeNull();
	}

	[Test]
	public void PatchUseCase_FieldAbsentFromJson_CanBeDetected()
	{
		// Arrange
		var json = """{"optionalString":"test"}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalInt.IsSpecified.Should().BeFalse("field was not present in JSON");
		result.OptionalInt.Value.Should().BeNull();
	}

	[Test]
	public void PatchUseCase_FieldExplicitlyNull_CanBeDetected()
	{
		// Arrange
		var json = """{"optionalString":"test","optionalInt":null}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalInt.IsSpecified.Should().BeTrue("field was present in JSON as null");
		result.OptionalInt.Value.Should().BeNull();
	}

	[Test]
	public void PatchUseCase_FieldWithValue_CanBeDetected()
	{
		// Arrange
		var json = """{"optionalString":"test","optionalInt":42}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(json);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalInt.IsSpecified.Should().BeTrue("field was present in JSON with value");
		result.OptionalInt.Value.Should().Be(42);
	}

	[Test]
	public void PatchScenario_WithExpiresOnFieldAbsent_ShouldNotUpdate()
	{
		// Arrange
		var patchRequest = """{"optionalString":"new-value"}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(patchRequest);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalDateTime.IsSpecified.Should().BeFalse("expiresOn field not in request → don't update");
	}

	[Test]
	public void PatchScenario_WithExpiresOnSetToNull_ShouldClearField()
	{
		// Arrange
		var patchRequest = """{"optionalString":"new-value","optionalDateTime":null}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(patchRequest);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalDateTime.IsSpecified.Should().BeTrue("expiresOn field present as null → clear it");
		result.OptionalDateTime.Value.Should().BeNull();
	}

	[Test]
	public void PatchScenario_WithExpiresOnSetToValue_ShouldUpdateField()
	{
		// Arrange
		var futureDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);
		var patchRequest = $$"""{"optionalString":"new-value","optionalDateTime":"{{futureDate:O}}"}""";

		// Act
		var result = JsonSerializer.Deserialize<TestModel>(patchRequest);

		// Assert
		result.Should().NotBeNull();
		result!.OptionalDateTime.IsSpecified.Should().BeTrue("expiresOn field present with value → update it");
		result.OptionalDateTime.Value.Should().Be(futureDate);
	}

	private class TestModel
	{
		[JsonPropertyName("optionalString")]
		[JsonConverter(typeof(OptionalValueJsonConverter<string>))]
		public OptionalValue<string> OptionalString { get; set; } = new();

		[JsonPropertyName("optionalInt")]
		[JsonConverter(typeof(OptionalValueJsonConverter<int?>))]
		public OptionalValue<int?> OptionalInt { get; set; } = new();

		[JsonPropertyName("optionalDateTime")]
		[JsonConverter(typeof(OptionalValueJsonConverter<DateTime?>))]
		public OptionalValue<DateTime?> OptionalDateTime { get; set; } = new();
	}
}
