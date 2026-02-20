using System.Text.Json.OptionalValue;

namespace ExampleApi.Models;

/// <summary>
/// PATCH request model for updating users.
/// Demonstrates GLOBAL REGISTRATION pattern - no attributes needed!
/// </summary>
public class PatchUserRequest
{
	/// <summary>
	/// User's display name. If not provided, name won't be updated.
	/// If null, name will be cleared (if allowed by business rules).
	/// </summary>
	public OptionalValue<string> Name { get; set; } = new();

	/// <summary>
	/// User's email address. If not provided, email won't be updated.
	/// </summary>
	public OptionalValue<string> Email { get; set; } = new();

	/// <summary>
	/// User's biography. If not provided, bio won't be updated.
	/// If null, bio will be cleared.
	/// </summary>
	public OptionalValue<string> Bio { get; set; } = new();

	/// <summary>
	/// Account expiration date. If not provided, expiration won't be updated.
	/// If null, account will never expire.
	/// </summary>
	public OptionalValue<DateTime?> ExpiresOn { get; set; } = new();

	/// <summary>
	/// Whether the user account is active. If not provided, status won't be updated.
	/// </summary>
	public OptionalValue<bool> IsActive { get; set; } = new();
}
