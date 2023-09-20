using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passwordless;

public sealed class Base64UrlConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!reader.HasValueSequence)
        {
            return Base64Url.DecodeUtf8(reader.ValueSpan);
        }
        return Base64Url.Decode(reader.GetString().AsSpan());
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Base64Url.Encode(value));
    }
}