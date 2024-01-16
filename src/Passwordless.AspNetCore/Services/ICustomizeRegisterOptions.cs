using System.Threading;
using System.Threading.Tasks;

namespace Passwordless.AspNetCore.Services;

public interface ICustomizeRegisterOptions
{
    Task CustomizeAsync(CustomizeRegisterOptionsContext context, CancellationToken cancellationToken);
}