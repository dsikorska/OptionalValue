using GlobalRegistrationApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GlobalRegistrationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
	private static readonly Dictionary<Guid, User> _store = new()
	{
		[Guid.Parse("11111111-1111-1111-1111-111111111111")] = new User
		{
			Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
			Name = "Alice",
			Bio = "Software engineer",
			UpdatedAt = DateTime.UtcNow
		},
		[Guid.Parse("22222222-2222-2222-2222-222222222222")] = new User
		{
			Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
			Name = "Bob",
			Bio = null,
			UpdatedAt = DateTime.UtcNow
		}
	};

	/// <summary>Get all users.</summary>
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
	public IActionResult GetAll() => Ok(_store.Values);

	/// <summary>Get a user by ID.</summary>
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(Guid id) =>
		_store.TryGetValue(id, out var user) ? Ok(user) : NotFound();

	/// <summary>
	/// Partially update a user (global registration pattern).
	/// </summary>
	/// <remarks>
	/// Because <c>AddOptionalValueSupport()</c> is registered globally in Program.cs,
	/// <c>OptionalValue&lt;T&gt;</c> properties deserialize correctly with no per-property
	/// attributes required.
	///
	/// Only fields present in the JSON body are updated; absent fields are left unchanged.
	///
	/// Update name only:
	/// <code>{ "name": "Alice Updated" }</code>
	///
	/// Clear bio (set to null):
	/// <code>{ "bio": null }</code>
	///
	/// Update both fields:
	/// <code>{ "name": "Alice", "bio": "New bio" }</code>
	/// </remarks>
	[HttpPatch("{id}")]
	[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Patch(Guid id, [FromBody] PatchUserRequest request)
	{
		if (!_store.TryGetValue(id, out var user))
			return NotFound();

		if (request.Name.IsSpecified)
			user.Name = request.Name.Value ?? string.Empty;

		if (request.Bio.IsSpecified)
			user.Bio = request.Bio.Value;

		user.UpdatedAt = DateTime.UtcNow;
		return Ok(user);
	}
}
