using Microsoft.AspNetCore.Mvc;
using PerPropertyAttributeApi.Models;

namespace PerPropertyAttributeApi.Controllers.Enums;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnumsController : ControllerBase
{
	private static readonly Dictionary<Guid, Priority?> _data = new()
	{
		[Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")] = Priority.Low,
		[Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")] = null
	};

	/// <summary>Get all items.</summary>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult GetAll() =>
		Ok(_data.Select(kv => new { Id = kv.Key, Priority = kv.Value }));

	/// <summary>Get an item by ID.</summary>
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(Guid id) =>
		_data.TryGetValue(id, out var priority)
			? Ok(new { Id = id, Priority = priority })
			: NotFound();

	/// <summary>
	/// Partially update an item (per-property attribute pattern — nullable enum).
	/// </summary>
	/// <remarks>
	/// <c>[JsonConverter(typeof(OptionalValueJsonConverter&lt;Priority?&gt;))]</c> on the
	/// property handles both "field absent" and "field present with null value" correctly.
	///
	/// Set priority:
	/// <code>{ "priority": "High" }</code>
	///
	/// Clear priority (set to null):
	/// <code>{ "priority": null }</code>
	///
	/// Leave priority unchanged (omit the field entirely):
	/// <code>{}</code>
	/// </remarks>
	[HttpPatch("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Patch(Guid id, [FromBody] PatchEnumsRequest request)
	{
		if (!_data.ContainsKey(id))
			return NotFound();

		if (request.Priority.IsSpecified)
			_data[id] = request.Priority.Value;

		return Ok(new { Id = id, Priority = _data[id] });
	}
}
