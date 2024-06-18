using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Passwordless.AspNetIdentity.Example.Pages.Authorized;

public class HelloWorldModel(ILogger<HelloWorldModel> logger, UserManager<IdentityUser> userManager) : PageModel
{
    public async Task OnGetAsync()
    {
        var identity = HttpContext.User.Identity!;
        var email = User.FindFirstValue(ClaimTypes.Email)!;
        AuthenticatedUser = new AuthenticatedUserModel(identity.Name!, email);
        Claims = (await userManager.GetClaimsAsync((await userManager.GetUserAsync(User))!))
            .Select(ClaimRecord.GetInstance)
            .Concat(User.Claims.Select(ClaimRecord.GetInstance));
    }

    public void OnPost(string? nickname)
    {
        if (!ModelState.IsValid) return;
        logger.LogInformation("Adding new credential for user {userName}", HttpContext.User.Identity!.Name);
        CanAddPassKeys = true;
        Nickname = nickname ?? HttpContext.User.Identity.Name;
    }

    public AuthenticatedUserModel? AuthenticatedUser { get; private set; }

    public string? Nickname { get; set; }

    public bool CanAddPassKeys { get; set; }

    public IEnumerable<ClaimRecord> Claims { get; set; } = ArraySegment<ClaimRecord>.Empty;
}

public record AuthenticatedUserModel(string Username, string Email);

public record ClaimRecord(string ClaimName, string ClaimValue)
{
    public static ClaimRecord GetInstance(Claim claim) => new(claim.Type, claim.Value);
};