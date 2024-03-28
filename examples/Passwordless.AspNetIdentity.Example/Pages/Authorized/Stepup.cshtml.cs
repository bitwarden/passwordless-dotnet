using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Passwordless.AspNetIdentity.Example.Pages.Authorized;

public class StepUp : PageModel
{
    public AuthenticatedUserModel? AuthenticatedUser { get; private set; }
    [BindProperty] public string RequestedContext { get; set; } = string.Empty;
    [BindProperty] public string ReturnUrl { get; set; } = string.Empty;
    [BindProperty] public string StepUpVerifyToken { get; set; } = string.Empty;

    public void OnGet(string context, string returnUrl)
    {
        var identity = HttpContext.User.Identity!;
        var email = User.FindFirstValue(ClaimTypes.Email)!;

        AuthenticatedUser = new AuthenticatedUserModel(identity.Name!, email);
        RequestedContext = context;
        ReturnUrl = returnUrl;
    }
}