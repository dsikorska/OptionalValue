using System.Text.Json.OptionalValue;
using System.Text.Json.Serialization;

namespace PerPropertyAttributeApi.Models;

public class PatchCollectionsRequest
{
	[JsonConverter(typeof(OptionalValueJsonConverter<List<string>?>))]
	public OptionalValue<List<string>?> Tags { get; set; } = new();
}
