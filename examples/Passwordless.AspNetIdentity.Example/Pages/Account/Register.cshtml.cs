using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Passwordless.AspNetIdentity.Example.Validation;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class RegisterModel(ILogger<PrivacyModel> logger) : PageModel
{
    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity is { IsAuthenticated: true })
        {
            return LocalRedirect("/");
        }

        return Page();
    }

    public Task OnPostAsync(FormModel form, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            logger.LogInformation("Registering user {username}", form.Username);
            ViewData["CanAddPasskeys"] = true;
        }

        return Task.CompletedTask;
    }

    public FormModel Form { get; init; } = new();
}

public class FormModel
{
    [Required]
    [MinLength(3)]
    [MaxLength(20)]
    [Alphanumeric]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
}