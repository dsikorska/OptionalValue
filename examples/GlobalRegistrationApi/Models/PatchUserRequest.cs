using System.Text.Json.OptionalValue;

namespace GlobalRegistrationApi.Models;

/// <summary>
/// PATCH request model for updating a user.
/// Demonstrates GLOBAL REGISTRATION — no [JsonConverter] attributes needed on properties.
/// <br/>
/// The <see cref="OptionalValueContractAttribute"/> below is documentation only; it has no
/// effect on serialization. Global setup (<c>AddOptionalValueSupport()</c> in Program.cs)
/// is what makes <see cref="OptionalValue{T}"/> work here.
/// </summary>
[OptionalValueContract]
public class PatchUserRequest
{
	/// <summary>User's display name. Omit to leave unchanged.</summary>
	public OptionalValue<string> Name { get; set; } = new();

	/// <summary>User's biography. Omit to leave unchanged. Send null to clear.</summary>
	public OptionalValue<string?> Bio { get; set; } = new();
}
