using System.Collections.Generic;

namespace Passwordless.AspNetCore.Services;

public record PasswordlessRegisterRequest(string Username, string? DisplayName, HashSet<string>? Aliases)
{
    public string? Email { get; set; }
}