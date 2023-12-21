using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity is { IsAuthenticated: true })
        {
            return LocalRedirect("/");
        }
        return Page(); 
    }

    public async Task OnPostAsync(LoginForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return;
        _logger.LogInformation("Logging in user {email}", form.Email);
        ViewData["CanLogin"] = true;
    }

    public LoginForm Form { get; } = new();
}

public class LoginForm
{
    [EmailAddress]
    [Required]
    public string? Email { get; set; }
}