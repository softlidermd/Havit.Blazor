using System.Text.Json;
using System.Text.Json.Serialization;

namespace Havit.Blazor.SoftLider.JSON;

public class NullableEnumConverter<TEnum> : JsonConverter<TEnum?> where TEnum : struct, Enum
{
	public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}

		string enumString = reader.GetString();
		if (Enum.TryParse(enumString, true, out TEnum result))
		{
			return result;
		}

		return null;
	}

	public override void Write(Utf8JsonWriter writer, TEnum? value, JsonSerializerOptions options)
	{
		if (value.HasValue)
		{
			writer.WriteStringValue(value.ToString());
		}
		else
		{
			writer.WriteNullValue();
		}
	}
}