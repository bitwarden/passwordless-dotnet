using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Passwordless.AspNetCore.Services;

public interface IPasswordlessService<TRegisterRequest>
{
    Task<IResult> RegisterUserAsync(TRegisterRequest request, CancellationToken cancellationToken);
    Task<IResult> LoginUserAsync(PasswordlessLoginRequest loginRequest, CancellationToken cancellationToken);
    Task<IResult> AddCredentialAsync(PasswordlessAddCredentialRequest request, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
}