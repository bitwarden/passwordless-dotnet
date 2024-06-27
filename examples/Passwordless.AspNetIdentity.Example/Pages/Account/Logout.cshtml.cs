using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class Logout(SignInManager<IdentityUser> userSignInManager, ILogger<Logout> logger) : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        var userManager = userSignInManager.UserManager;

        var user = await userManager.GetUserAsync(User);
        if (user is not null) await userManager.RemoveClaimsAsync(user, await userManager.GetClaimsAsync(user));

        await userSignInManager.SignOutAsync();
        logger.LogInformation("User has signed out.");

        return Page();
    }
}