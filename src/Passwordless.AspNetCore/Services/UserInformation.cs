using System.Collections.Generic;

namespace Passwordless.AspNetCore.Services;

public sealed record UserInformation(string Username, string? DisplayName, HashSet<string>? Aliases);