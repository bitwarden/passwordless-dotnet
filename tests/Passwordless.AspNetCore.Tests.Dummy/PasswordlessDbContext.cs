using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Passwordless.AspNetCore.Tests.Dummy;

public class PasswordlessDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public PasswordlessDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite($"Data Source=database-{Guid.NewGuid():N}.db");
    }
}