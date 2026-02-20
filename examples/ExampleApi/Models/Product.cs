namespace ExampleApi.Models;

/// <summary>
/// Represents a product in the system.
/// </summary>
public class Product
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public decimal Price { get; set; }
	public int Stock { get; set; }
	public string? Category { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
