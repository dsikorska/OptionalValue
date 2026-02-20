using Microsoft.AspNetCore.Mvc;
using PerPropertyAttributeApi.Models;

namespace PerPropertyAttributeApi.Controllers.Collections;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CollectionsController : ControllerBase
{
	private static readonly Dictionary<Guid, List<string>?> _data = new()
	{
		[Guid.Parse("11111111-2222-3333-4444-555555555555")] = new List<string> { "dotnet", "csharp" },
		[Guid.Parse("66666666-7777-8888-9999-aaaaaaaaaaaa")] = null
	};

	/// <summary>Get all items.</summary>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult GetAll() =>
		Ok(_data.Select(kv => new { Id = kv.Key, Tags = kv.Value }));

	/// <summary>Get an item by ID.</summary>
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(Guid id) =>
		_data.TryGetValue(id, out var tags)
			? Ok(new { Id = id, Tags = tags })
			: NotFound();

	/// <summary>
	/// Partially update an item (per-property attribute pattern — collection).
	/// </summary>
	/// <remarks>
	/// <c>[JsonConverter(typeof(OptionalValueJsonConverter&lt;List&lt;string&gt;?&gt;))]</c>
	/// correctly distinguishes between "tags not provided" and "tags set to null".
	///
	/// Replace tags:
	/// <code>{ "tags": ["api", "rest"] }</code>
	///
	/// Clear tags:
	/// <code>{ "tags": null }</code>
	///
	/// Leave tags unchanged:
	/// <code>{}</code>
	/// </remarks>
	[HttpPatch("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Patch(Guid id, [FromBody] PatchCollectionsRequest request)
	{
		if (!_data.ContainsKey(id))
			return NotFound();

		if (request.Tags.IsSpecified)
			_data[id] = request.Tags.Value;

		return Ok(new { Id = id, Tags = _data[id] });
	}
}
