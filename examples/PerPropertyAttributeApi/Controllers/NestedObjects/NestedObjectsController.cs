using Microsoft.AspNetCore.Mvc;
using PerPropertyAttributeApi.Models;

namespace PerPropertyAttributeApi.Controllers.NestedObjects;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NestedObjectsController : ControllerBase
{
	private static readonly Dictionary<Guid, Address?> _data = new()
	{
		[Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")] = new Address { Street = "123 Main St", City = "Springfield" },
		[Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")] = null
	};

	/// <summary>Get all items.</summary>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult GetAll() =>
		Ok(_data.Select(kv => new { Id = kv.Key, Address = kv.Value }));

	/// <summary>Get an item by ID.</summary>
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(Guid id) =>
		_data.TryGetValue(id, out var address)
			? Ok(new { Id = id, Address = address })
			: NotFound();

	/// <summary>
	/// Partially update an item (per-property attribute pattern — complex object).
	/// </summary>
	/// <remarks>
	/// <c>[JsonConverter(typeof(OptionalValueJsonConverter&lt;Address?&gt;))]</c> correctly
	/// distinguishes between "address not provided" and "address set to null".
	///
	/// Set address:
	/// <code>{ "address": { "street": "456 Oak Ave", "city": "Shelbyville" } }</code>
	///
	/// Clear address:
	/// <code>{ "address": null }</code>
	///
	/// Leave address unchanged:
	/// <code>{}</code>
	/// </remarks>
	[HttpPatch("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Patch(Guid id, [FromBody] PatchNestedObjectRequest request)
	{
		if (!_data.ContainsKey(id))
			return NotFound();

		if (request.Address.IsSpecified)
			_data[id] = request.Address.Value;

		return Ok(new { Id = id, Address = _data[id] });
	}
}
