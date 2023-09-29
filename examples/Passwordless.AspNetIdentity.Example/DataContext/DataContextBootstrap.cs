using Microsoft.Extensions.DependencyInjection;

namespace Passwordless.DataContext;

public static class DataContextBootstrap
{
    public static void AddDataContext(this IServiceCollection services)
    {
        services.AddDbContext<PasswordlessContext>();
    }
}