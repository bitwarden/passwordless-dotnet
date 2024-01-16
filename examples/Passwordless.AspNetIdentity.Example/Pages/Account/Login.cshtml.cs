using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class LoginModel(ILogger<LoginModel> logger) : PageModel
{
    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity is { IsAuthenticated: true })
        {
            return LocalRedirect("/");
        }
        return Page();
    }

    public Task OnPostAsync(LoginForm form, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            logger.LogInformation("Logging in user {email}", form.Email);
            ViewData["CanLogin"] = true;
        }

        return Task.CompletedTask;
    }

    public LoginForm Form { get; } = new();
}

public class LoginForm
{
    [EmailAddress]
    [Required]
    public string? Email { get; set; }
}