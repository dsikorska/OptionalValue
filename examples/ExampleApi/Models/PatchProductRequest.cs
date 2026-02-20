using System.Text.Json.OptionalValue;

namespace ExampleApi.Models;

/// <summary>
/// PATCH request model for updating products.
/// Demonstrates PER-CLASS ATTRIBUTE pattern - documents intent with [OptionalValueContract].
/// </summary>
[OptionalValueContract]
public class PatchProductRequest
{
	/// <summary>
	/// Product name. If not provided, name won't be updated.
	/// </summary>
	public OptionalValue<string> Name { get; set; } = new();

	/// <summary>
	/// Product description. If not provided, description won't be updated.
	/// If null, description will be cleared.
	/// </summary>
	public OptionalValue<string> Description { get; set; } = new();

	/// <summary>
	/// Product price. If not provided, price won't be updated.
	/// </summary>
	public OptionalValue<decimal?> Price { get; set; } = new();

	/// <summary>
	/// Stock quantity. If not provided, stock won't be updated.
	/// </summary>
	public OptionalValue<int?> Stock { get; set; } = new();

	/// <summary>
	/// Product category. If not provided, category won't be updated.
	/// If null, category will be cleared.
	/// </summary>
	public OptionalValue<string> Category { get; set; } = new();
}
