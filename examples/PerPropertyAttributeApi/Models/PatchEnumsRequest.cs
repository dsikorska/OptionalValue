using System.Text.Json.OptionalValue;
using System.Text.Json.Serialization;

namespace PerPropertyAttributeApi.Models;

public class PatchEnumsRequest
{
	[JsonConverter(typeof(OptionalValueJsonConverter<Priority?>))]
	public OptionalValue<Priority?> Priority { get; set; } = new();
}
