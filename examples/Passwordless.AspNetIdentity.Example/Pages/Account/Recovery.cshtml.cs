using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Passwordless.Models;

namespace Passwordless.AspNetIdentity.Example.Pages.Account;

public class Recovery : PageModel
{
    private readonly ILogger<Recovery> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly PasswordlessClient _passwordlessClient;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IActionContextAccessor _actionContextAccessor;

    public const string MagicLink = "magic_link";
    public const string GeneratedSignIn = "generated_signin";

    public RecoveryForm Form { get; } = new();

    public string RecoveryMessage { get; set; } = string.Empty;

    public Recovery(ILogger<Recovery> logger,
        SignInManager<IdentityUser> signInManager,
        PasswordlessClient passwordlessClient,
        IUrlHelperFactory urlHelperFactory,
        IActionContextAccessor actionContextAccessor)
    {
        _logger = logger;
        _signInManager = signInManager;
        _passwordlessClient = passwordlessClient;
        _urlHelperFactory = urlHelperFactory;
        _actionContextAccessor = actionContextAccessor;
    }

    public void OnGetSuccessfulRecovery(string message)
    {
        if (!string.IsNullOrWhiteSpace(message)) RecoveryMessage = message;
    }

    public async Task<IActionResult> OnPostAsync(RecoveryForm form, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();

        var user = await _signInManager.UserManager.FindByEmailAsync(form.Email!);

        if (user == null) return Page();

        _logger.LogInformation("Sending magic link.");

        if (_actionContextAccessor.ActionContext == null)
        {
            _logger.LogError("ActionContext is null");
            throw new InvalidOperationException("ActionContext is null");
        }

        var urlBuilder = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var url = urlBuilder.PageLink("/Account/Magic") ?? urlBuilder.Content("~/");

        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["token"] = string.Empty;
        uriBuilder.Query = query.ToString();
        var successMessage = string.Empty;

        if (string.Equals(form.RecoveryMethod, MagicLink, StringComparison.OrdinalIgnoreCase))
        {
            await _passwordlessClient.SendMagicLinkAsync(new SendMagicLinkRequest(form.Email!, uriBuilder.Uri + "$TOKEN", user.Id, null), cancellationToken);

            successMessage = "Check your API magic link destination (mail.md if running local, email if using cloud.";
        }
        else if (string.Equals(form.RecoveryMethod, GeneratedSignIn, StringComparison.OrdinalIgnoreCase))
        {
            var token = await _passwordlessClient.GenerateAuthenticationTokenAsync(new AuthenticationOptions(user.Id), cancellationToken);
            query["token"] = token.Token;
            uriBuilder.Query = query.ToString();

            var message = $"""
                           New message:

                           This was generated with manually generated authentication token.
                           <a href="{uriBuilder}">Link<a>
                           {Environment.NewLine}
                           """;

            await System.IO.File.AppendAllTextAsync("mail.md", message, cancellationToken);

            successMessage = message;
        }

        return RedirectToPage("Recovery", "SuccessfulRecovery", new { message = successMessage });
    }
}

public class RecoveryForm
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string RecoveryMethod { get; set; } = string.Empty;
}