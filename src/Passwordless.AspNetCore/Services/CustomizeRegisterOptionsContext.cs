namespace Passwordless.AspNetCore.Services;

public sealed record CustomizeRegisterOptionsContext(bool NewUser, RegisterOptions Options)
{
    // Why is this needed?
    public RegisterOptions? Options { get; set; } = Options;
}