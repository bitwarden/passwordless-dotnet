using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Passwordless.DataContext;

public class PasswordlessContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public PasswordlessContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite("Data Source=example.db");
    }
}