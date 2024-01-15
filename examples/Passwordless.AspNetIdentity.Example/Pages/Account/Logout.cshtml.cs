using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class Logout(SignInManager<IdentityUser> userSignInManager, ILogger<Logout> logger) : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        await userSignInManager.SignOutAsync();
        logger.LogInformation("User has signed out.");

        return Page();
    }
}