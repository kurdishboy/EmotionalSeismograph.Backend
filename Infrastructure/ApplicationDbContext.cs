using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, long>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(b =>
        {
            b.ToTable("Users");
        });

        builder.Entity<IdentityUserClaim<long>>(b =>
        {
            b.ToTable("UserClaims");
        });

        builder.Entity<IdentityUserLogin<long>>(b =>
        {
            b.ToTable("UserLogins");
        });

        builder.Entity<IdentityUserToken<long>>(b =>
        {
            b.ToTable("UserTokens");
        });

        builder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
        });

        builder.Entity<IdentityRoleClaim<long>>(b =>
        {
            b.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserRole<long>>(b =>
        {
            b.ToTable("UserRoles");
        });
    }
}