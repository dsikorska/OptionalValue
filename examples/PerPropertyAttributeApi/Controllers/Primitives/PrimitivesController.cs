using Microsoft.AspNetCore.Mvc;
using PerPropertyAttributeApi.Models;

namespace PerPropertyAttributeApi.Controllers.Primitives;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PrimitivesController : ControllerBase
{
	private static readonly Dictionary<Guid, (string Name, bool IsPublished)> _data = new()
	{
		[Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")] = ("Draft post", false),
		[Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")] = ("Published post", true)
	};

	/// <summary>Get all items.</summary>
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IActionResult GetAll() =>
		Ok(_data.Select(kv => new { Id = kv.Key, Name = kv.Value.Name, IsPublished = kv.Value.IsPublished }));

	/// <summary>Get an item by ID.</summary>
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult GetById(Guid id) =>
		_data.TryGetValue(id, out var item)
			? Ok(new { Id = id, Name = item.Name, IsPublished = item.IsPublished })
			: NotFound();

	/// <summary>
	/// Partially update an item (per-property attribute pattern — string + bool).
	/// </summary>
	/// <remarks>
	/// Each <c>OptionalValue&lt;T&gt;</c> property carries a
	/// <c>[JsonConverter(typeof(OptionalValueJsonConverter&lt;T&gt;))]</c> attribute.
	/// No global setup is needed — the converter is resolved per-property by the serializer.
	///
	/// Update name only:
	/// <code>{ "name": "Updated name" }</code>
	///
	/// Toggle publish flag only:
	/// <code>{ "isPublished": true }</code>
	/// </remarks>
	[HttpPatch("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult Patch(Guid id, [FromBody] PatchPrimitivesRequest request)
	{
		if (!_data.TryGetValue(id, out var item))
			return NotFound();

		var (name, isPublished) = item;

		if (request.Name.IsSpecified)
			name = request.Name.Value ?? string.Empty;

		if (request.IsPublished.IsSpecified)
			isPublished = request.IsPublished.Value;

		_data[id] = (name, isPublished);
		return Ok(new { Id = id, Name = name, IsPublished = isPublished });
	}
}
