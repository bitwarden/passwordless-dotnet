using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Passwordless.AspNetIdentity.Example.DataContext;

public class PasswordlessContext(DbContextOptions options) :
    IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite("Data Source=example.db");
    }
}