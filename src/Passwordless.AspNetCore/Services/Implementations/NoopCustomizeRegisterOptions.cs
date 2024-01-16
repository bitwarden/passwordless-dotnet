using System.Threading;
using System.Threading.Tasks;

namespace Passwordless.AspNetCore.Services.Implementations;

internal sealed class NoopCustomizeRegisterOptions : ICustomizeRegisterOptions
{
    public Task CustomizeAsync(CustomizeRegisterOptionsContext context, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}