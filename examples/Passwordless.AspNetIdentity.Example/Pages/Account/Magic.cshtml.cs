using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class Magic : PageModel
{
    private readonly PasswordlessClient _passwordlessClient;
    private readonly SignInManager<IdentityUser> _signInManager;

    public Magic(PasswordlessClient passwordlessClient,
        SignInManager<IdentityUser> signInManager)
    {
        _passwordlessClient = passwordlessClient;
        _signInManager = signInManager;
    }

    public bool Success { get; set; }

    private static readonly string[] GeneratedTokenTypes = ["magic_link", "generated_signin"];

    public async Task<ActionResult> OnGet(string token)
    {
        if (User.Identity is { IsAuthenticated: true }) return RedirectToPage("/Authorized/HelloWorld");

        if (string.IsNullOrWhiteSpace(token))
        {
            Success = false;
            return Page();
        }

        var response = await _passwordlessClient.VerifyAuthenticationTokenAsync(token);

        if (!response.Success)
        {
            Success = false;
            return Page();
        }

        var user = await _signInManager.UserManager.FindByIdAsync(response.UserId);

        if (user == null || !(await _signInManager.CanSignInAsync(user)))
        {
            Success = false;
            return Page();
        }

        await _signInManager.SignInAsync(user, true);


        if (GeneratedTokenTypes.Contains(response.Type))
        {
            await _signInManager.UserManager.AddClaimAsync(user, new Claim(response.Type, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));
        }

        Success = true;
        return LocalRedirect("/Authorized/HelloWorld");
    }
}