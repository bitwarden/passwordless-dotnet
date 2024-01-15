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

    public AuthenticatedUserModel? AuthenticatedUser { get; private set; }
}

public record AuthenticatedUserModel(string Username, string Email);