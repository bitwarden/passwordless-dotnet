using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Passwordless.Models;

namespace Passwordless.Helpers;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(RegisterTokenResponse))]
[JsonSerializable(typeof(RegisterOptions))]
[JsonSerializable(typeof(AuthenticationTokenResponse))]
[JsonSerializable(typeof(AuthenticationOptions))]
[JsonSerializable(typeof(VerifyTokenRequest))]
[JsonSerializable(typeof(VerifiedUser))]
[JsonSerializable(typeof(DeleteUserRequest))]
[JsonSerializable(typeof(ListResponse<PasswordlessUserSummary>))]
[JsonSerializable(typeof(SetAliasRequest))]
[JsonSerializable(typeof(ListResponse<AliasPointer>))]
[JsonSerializable(typeof(ListResponse<Credential>))]
[JsonSerializable(typeof(DeleteCredentialRequest))]
[JsonSerializable(typeof(UsersCount))]
[JsonSerializable(typeof(PasswordlessProblemDetails))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(GetEventLogRequest))]
[JsonSerializable(typeof(GetEventLogResponse))]
internal partial class PasswordlessSerializerContext : JsonSerializerContext
{

}