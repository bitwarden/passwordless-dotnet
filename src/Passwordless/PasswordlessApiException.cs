using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passwordless;

public sealed class PasswordlessApiException : HttpRequestException
{
    public PasswordlessProblemDetails Details { get; }

    public PasswordlessApiException(PasswordlessProblemDetails problemDetails) : base(problemDetails.Title)
    {
        Details = problemDetails;
    }
}

public class PasswordlessProblemDetails
{
    public PasswordlessProblemDetails(string type,
        string title, int status, string? detail, string? instance)
    {
        Type = type;
        Title = title;
        Status = status;
        Detail = detail;
        Instance = instance;
    }

    // TODO: Include errorCode as a property once it's more common
    public string Type { get; }
    public string Title { get; }
    public int Status { get; }
    public string? Detail { get; }
    public string? Instance { get; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> Extensions { get; set; } = new Dictionary<string, JsonElement>();
}