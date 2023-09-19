using System.Text.Json;
using System.Text.Json.Serialization;
using Passwordless.Net.Models;

namespace Passwordless.Net.Helpers;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AddAliasRequest))]
[JsonSerializable(typeof(RegisterTokenResponse))]
[JsonSerializable(typeof(RegisterOptions))]
[JsonSerializable(typeof(VerifyTokenRequest))] // TODO: Use this with JsonContent.Create
[JsonSerializable(typeof(VerifiedUser))]
[JsonSerializable(typeof(DeleteUserRequest))]
[JsonSerializable(typeof(ListResponse<PasswordlessUserSummary>))]
[JsonSerializable(typeof(ListResponse<AliasPointer>))]
[JsonSerializable(typeof(ListResponse<Credential>))]
[JsonSerializable(typeof(DeleteCredentialRequest))]
[JsonSerializable(typeof(UsersCount))]
[JsonSerializable(typeof(PasswordlessProblemDetails))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(JsonElement))]
internal partial class PasswordlessSerializerContext : JsonSerializerContext
{

}