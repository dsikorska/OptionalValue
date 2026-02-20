using System.Text.Json.OptionalValue;
using System.Text.Json.Serialization;

namespace PerPropertyAttributeApi.Models;

public class PatchNestedObjectRequest
{
	[JsonConverter(typeof(OptionalValueJsonConverter<Address?>))]
	public OptionalValue<Address?> Address { get; set; } = new();
}
