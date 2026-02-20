using System.Text.Json.OptionalValue;
using System.Text.Json.Serialization;

namespace PerPropertyAttributeApi.Models;

public class PatchPrimitivesRequest
{
	[JsonConverter(typeof(OptionalValueJsonConverter<string>))]
	public OptionalValue<string> Name { get; set; } = new();

	[JsonConverter(typeof(OptionalValueJsonConverter<bool>))]
	public OptionalValue<bool> IsPublished { get; set; } = new();
}
