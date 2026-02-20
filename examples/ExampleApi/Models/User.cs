namespace ExampleApi.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string? Bio { get; set; }
	public DateTime? ExpiresOn { get; set; }
	public bool IsActive { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
