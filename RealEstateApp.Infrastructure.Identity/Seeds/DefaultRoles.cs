using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds;

public static class DefaultRoles
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        if (roleManager.Roles.Any()) return;

        string[] roles = Enum.GetNames<Roles>();
        foreach (var role in roles)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

public static class DefaultUsers
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        if (userManager.Users.Any()) return;

        var admin = new ApplicationUser
        {
            FirstName = "Admin",
            LastName = "Sistema",
            IdCard = "00000000001",
            UserName = "admin",
            Email = "admin@realestateapp.com",
            EmailConfirmed = true,
            IsActive = true
        };
        await CreateIfNotExists(userManager, admin, "Admin123$", Roles.Administrador.ToString());

        var client = new ApplicationUser
        {
            FirstName = "Cliente",
            LastName = "Demo",
            IdCard = "00000000002",
            UserName = "cliente",
            Email = "cliente@realestateapp.com",
            EmailConfirmed = true,
            IsActive = true
        };
        await CreateIfNotExists(userManager, client, "Cliente123$", Roles.Cliente.ToString());

        var agent = new ApplicationUser
        {
            FirstName = "Agente",
            LastName = "Demo",
            IdCard = "00000000003",
            UserName = "agente",
            Email = "agente@realestateapp.com",
            EmailConfirmed = true,
            IsActive = true
        };
        await CreateIfNotExists(userManager, agent, "Agente123$", Roles.Agente.ToString());

        var developer = new ApplicationUser
        {
            FirstName = "Developer",
            LastName = "Demo",
            IdCard = "00000000004",
            UserName = "developer",
            Email = "developer@realestateapp.com",
            EmailConfirmed = true,
            IsActive = true
        };
        await CreateIfNotExists(userManager, developer, "Developer123$", Roles.Desarrollador.ToString());
    }

    private static async Task CreateIfNotExists( UserManager<ApplicationUser> userManager,
        ApplicationUser user, string password, string role)
    {
        if (await userManager.FindByNameAsync(user.UserName!) is not null)
            return;

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);
    }
}