using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passwordless;

/// <summary>
/// A problem details contract as defined in RFC 7807.
/// </summary>
public record PasswordlessProblemDetails(
    string Type,
    string Title,
    int Status,
    string? Detail,
    string? Instance)
{
    /// <summary>
    /// Additional domain-specific details about the problem.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Extensions { get; set; } = new();
}