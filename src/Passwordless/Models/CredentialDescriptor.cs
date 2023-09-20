using System.Text.Json.Serialization;

namespace Passwordless;

public class CredentialDescriptor
{
    public CredentialDescriptor(byte[] id)
    {
        Id = id;
    }

    [JsonConverter(typeof(Base64UrlConverter))]
    public byte[] Id { get; set; }
}