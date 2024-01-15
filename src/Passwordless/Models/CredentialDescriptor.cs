using System.Text.Json.Serialization;

namespace Passwordless;

public record CredentialDescriptor([property: JsonConverter(typeof(Base64UrlConverter))] byte[] Id);