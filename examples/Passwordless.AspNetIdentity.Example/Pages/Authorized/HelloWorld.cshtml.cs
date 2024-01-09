using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Passwordless.AspNetIdentity.Example.Pages.Authorized;

public class HelloWorldModel : PageModel
{
    private readonly ILogger<HelloWorldModel> _logger;

    public HelloWorldModel(ILogger<HelloWorldModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet()
    {
        var identity = HttpContext.User.Identity!;
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        AuthenticatedUser = new AuthenticatedUserModel(identity.Name!, email);
    }

    public void OnPost(string? nickname)
    {
        if (!ModelState.IsValid) return;
        _logger.LogInformation("Adding new credential for user {userName}", HttpContext.User.Identity!.Name);
        CanAddPassKeys = true;
        Nickname = nickname ?? HttpContext.User.Identity.Name;
    }

    public AuthenticatedUserModel? AuthenticatedUser { get; private set; }

    public string? Nickname { get; set; }

    public bool CanAddPassKeys { get; set; }
}

public record AuthenticatedUserModel(string Username, string Email);