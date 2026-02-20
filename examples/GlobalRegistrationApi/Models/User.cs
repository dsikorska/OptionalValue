namespace GlobalRegistrationApi.Models;

public class User
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string? Bio { get; set; }
	public DateTime UpdatedAt { get; set; }
}
