using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passwordless;

/// <summary>
/// encode/decode byte array to/from base64url encoded string
/// </summary>
public sealed class Base64UrlConverter : JsonConverter<byte[]>
{
    /// <summary>
    /// decode base64url encoded string to byte array
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!reader.HasValueSequence)
        {
            return Base64Url.DecodeUtf8(reader.ValueSpan);
        }
        return Base64Url.Decode(reader.GetString().AsSpan());
    }

    /// <summary>
    /// encode byte array to base64url encoded string
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(Base64Url.Encode(value));
    }
}