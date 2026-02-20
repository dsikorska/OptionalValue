using FluentAssertions;
using NUnit.Framework;

namespace System.Text.Json.OptionalValue.Tests;

[TestFixture]
public class OptionalValueTests
{
	[Test]
	public void Constructor_WithDefaultConstructor_ShouldHaveIsSpecifiedFalse()
	{
		// Arrange & Act
		var optional = new OptionalValue<string>();

		// Assert
		optional.IsSpecified.Should().BeFalse();
		optional.Value.Should().BeNull();
	}

	[Test]
	public void Constructor_WithValue_ShouldHaveIsSpecifiedTrue()
	{
		// Arrange & Act
		var optional = new OptionalValue<string>("test-value");

		// Assert
		optional.IsSpecified.Should().BeTrue();
		optional.Value.Should().Be("test-value");
	}

	[Test]
	public void Constructor_WithNull_ShouldHaveIsSpecifiedTrue()
	{
		// Arrange & Act
		var optional = new OptionalValue<string>(null);

		// Assert
		optional.IsSpecified.Should().BeTrue();
		optional.Value.Should().BeNull();
	}

	[Test]
	public void ImplicitOperator_FromValue_ShouldCreateOptionalWithIsSpecifiedTrue()
	{
		// Arrange & Act
		OptionalValue<int> optional = 42;

		// Assert
		optional.IsSpecified.Should().BeTrue();
		optional.Value.Should().Be(42);
	}

	[Test]
	public void ImplicitOperator_FromNull_ShouldCreateOptionalWithIsSpecifiedTrue()
	{
		// Arrange
		string? nullValue = null;

		// Act - Implicit operator should be called when assigning string? to OptionalValue<string>
		OptionalValue<string> optional = nullValue;

		// Assert
		optional.Should().NotBeNull();
		optional.IsSpecified.Should().BeTrue();
		optional.Value.Should().BeNull();
	}

	[Test]
	public void ImplicitOperator_ToValue_ShouldReturnUnderlyingValue()
	{
		// Arrange
		var optional = new OptionalValue<string>("test-value");

		// Act
		string? value = optional;

		// Assert
		value.Should().Be("test-value");
	}

	[Test]
	public void ImplicitOperator_ToValue_WithNull_ShouldReturnNull()
	{
		// Arrange
		var optional = new OptionalValue<string>(null);

		// Act
		string? value = optional;

		// Assert
		value.Should().BeNull();
	}

	[Test]
	public void OptionalValue_WithDateTime_ShouldWorkCorrectly()
	{
		// Arrange
		var date = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);

		// Act
		var optional = new OptionalValue<DateTime?>(date);

		// Assert
		optional.IsSpecified.Should().BeTrue();
		optional.Value.Should().Be(date);
	}

	[Test]
	public void OptionalValue_WithNullableDateTime_SetToNull_ShouldHaveIsSpecifiedTrue()
	{
		// Arrange & Act
		var optional = new OptionalValue<DateTime?>(null);

		// Assert
		optional.IsSpecified.Should().BeTrue();
		optional.Value.Should().BeNull();
	}
}
