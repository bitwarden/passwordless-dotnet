using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class Logout : PageModel
{
    private readonly SignInManager<IdentityUser> _userSignInManager;
    private readonly ILogger<Logout> _logger;

    public Logout(SignInManager<IdentityUser> userSignInManager, ILogger<Logout> logger)
    {
        _userSignInManager = userSignInManager;
        _logger = logger;
    }
    
    public async Task<IActionResult> OnGet()
    {
        await _userSignInManager.SignOutAsync();
        _logger.LogInformation("User has signed out.");
        return Page();
    }
}