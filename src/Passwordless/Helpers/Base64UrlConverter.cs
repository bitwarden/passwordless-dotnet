using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passwordless;

/// <summary>
/// Encode/decode byte arrays to/from Base64 URL-encoded strings
/// </summary>
public sealed class Base64UrlConverter : JsonConverter<byte[]>
{
    /// <summary>
    /// Decode a Base64 URL-encoded string into a byte array.
    /// </summary>
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        !reader.HasValueSequence
            ? Base64Url.DecodeUtf8(reader.ValueSpan)
            : Base64Url.Decode(reader.GetString().AsSpan());

    /// <summary>
    /// Encode a byte array into a Base64 URL-encoded string.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options) =>
        writer.WriteStringValue(Base64Url.Encode(value));
}