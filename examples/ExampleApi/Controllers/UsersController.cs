using ExampleApi.Models;
using ExampleApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;
	private readonly ILogger<UsersController> _logger;

	public UsersController(IUserService userService, ILogger<UsersController> logger)
	{
		_userService = userService;
		_logger = logger;
	}

	/// <summary>
	/// Get all users.
	/// </summary>
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
	public IActionResult GetAll()
	{
		var users = _userService.GetAll();
		return Ok(users);
	}

	/// <summary>
	/// Get a user by ID.
	/// </summary>
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(Guid id)
	{
		var user = _userService.GetById(id);
		if (user == null)
		{
			return NotFound(new { message = $"User with ID {id} not found" });
		}

		return Ok(user);
	}

	/// <summary>
	/// Partially update a user using PATCH.
	/// Only the fields provided in the request will be updated.
	/// </summary>
	/// <remarks>
	/// This endpoint demonstrates the OptionalValue&lt;T&gt; pattern with GLOBAL REGISTRATION.
	///
	/// Example requests:
	///
	/// 1. Update only name:
	/// ```json
	/// {
	///   "name": "John Updated"
	/// }
	/// ```
	///
	/// 2. Clear bio (set to null):
	/// ```json
	/// {
	///   "bio": null
	/// }
	/// ```
	///
	/// 3. Update multiple fields:
	/// ```json
	/// {
	///   "name": "Jane Smith",
	///   "email": "jane.smith@example.com",
	///   "bio": "Product Manager",
	///   "expiresOn": "2026-12-31T23:59:59Z",
	///   "isActive": true
	/// }
	/// ```
	///
	/// 4. Remove expiration (set to null):
	/// ```json
	/// {
	///   "expiresOn": null
	/// }
	/// ```
	/// </remarks>
	[HttpPatch("{id}")]
	[ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Patch(Guid id, [FromBody] PatchUserRequest request)
	{
		_logger.LogInformation("PATCH /api/users/{Id} - Received request", id);

		// Log which fields were provided
		_logger.LogInformation(
			"Fields specified: Name={NameSpecified}, Email={EmailSpecified}, Bio={BioSpecified}, ExpiresOn={ExpiresOnSpecified}, IsActive={IsActiveSpecified}",
			request.Name.IsSpecified,
			request.Email.IsSpecified,
			request.Bio.IsSpecified,
			request.ExpiresOn.IsSpecified,
			request.IsActive.IsSpecified
		);

		var user = _userService.Update(id, request);
		if (user == null)
		{
			return NotFound(new { message = $"User with ID {id} not found" });
		}

		return Ok(user);
	}

	/// <summary>
	/// Delete a user.
	/// </summary>
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Delete(Guid id)
	{
		var deleted = _userService.Delete(id);
		if (!deleted)
		{
			return NotFound(new { message = $"User with ID {id} not found" });
		}

		return NoContent();
	}
}
