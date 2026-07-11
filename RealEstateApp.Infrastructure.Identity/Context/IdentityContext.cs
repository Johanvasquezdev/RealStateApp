using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Context;

public class IdentityContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("Identity");
        builder.Entity<ApplicationUser>().ToTable(name: "Users");
        builder.Entity<IdentityRole>().ToTable(name: "Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable(name: "UserRoles");
        builder.Entity<IdentityUserLogin<string>>().ToTable(name: "UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable(name: "UserTokens");
        builder.Entity<IdentityRoleClaim<string>>().ToTable(name: "RoleClaims");
        builder.Entity<IdentityUserClaim<string>>().ToTable(name: "UserClaims");
    }
}
