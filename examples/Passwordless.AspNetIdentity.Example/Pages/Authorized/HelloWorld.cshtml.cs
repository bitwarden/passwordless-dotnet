using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Passwordless.AspNetIdentity.Example.Pages.Authorized;

public class HelloWorldModel : PageModel
{
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