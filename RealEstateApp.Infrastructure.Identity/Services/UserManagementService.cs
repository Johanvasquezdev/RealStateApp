using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Users;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class UserManagementService( UserManager<ApplicationUser> userManager, IGenericRepository<Property> propertyRepository) : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IGenericRepository<Property> _propertyRepository = propertyRepository;

    private const string RoleAdmin = "Administrador";
    private const string RoleDeveloper = "Desarrollador";
    private const string RoleAgent = "Agente";
    private const string RoleClient = "Cliente";

    private async Task<List<UserListItemViewModel>> GetByRole(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        return users.Select(u => new UserListItemViewModel
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email!,
            UserName = u.UserName!,
            IdCard = u.IdCard,
            IsActive = u.IsActive
        }).ToList();
    }

    public Task<List<UserListItemViewModel>> GetAllAdmins() => GetByRole(RoleAdmin);
    public Task<List<UserListItemViewModel>> GetAllDevelopers() => GetByRole(RoleDeveloper);

    public async Task<List<UserListItemViewModel>> GetAllAgents()
    {
        var agents = await GetByRole(RoleAgent);

        foreach (var agent in agents)
        {
            var agentProperties = await _propertyRepository.FindAsync(p => p.AgentId == agent.Id);
            agent.PropertiesCount = agentProperties.Count;
        }

        return agents;
    }

    private async Task<(bool, string[])> CreateUserWithRole(SaveAdminViewModel vm, string role)
    {
        if (vm.Password != vm.ConfirmPassword)
            return (false, new[] { "las claves no coinciden." });

        var user = new ApplicationUser
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            IdCard = vm.IdCard,
            Email = vm.Email,
            UserName = vm.UserName,
            IsActive = true,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, vm.Password!);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        await _userManager.AddToRoleAsync(user, role);
        return (true, Array.Empty<string>());
    }

    public Task<(bool Succeeded, string[] Errors)> CreateAdmin(SaveAdminViewModel vm) =>
        CreateUserWithRole(vm, RoleAdmin);

    public Task<(bool Succeeded, string[] Errors)> CreateDeveloper(SaveDeveloperViewModel vm) =>
        CreateUserWithRole(vm, RoleDeveloper);

    private async Task<(bool, string[])> UpdateUser(SaveAdminViewModel vm)
    {
        var user = await _userManager.FindByIdAsync(vm.Id!);
        if (user is null)
            return (false, new[] { "El usuario solicitado no existe." });

        user.FirstName = vm.FirstName;
        user.LastName = vm.LastName;
        user.IdCard = vm.IdCard;
        user.Email = vm.Email;
        user.UserName = vm.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description).ToArray());

        if (!string.IsNullOrWhiteSpace(vm.Password))
        {
            if (vm.Password != vm.ConfirmPassword)
                return (false, new[] { "La contraseña y la confirmación de contraseña no coinciden." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var pwResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
            if (!pwResult.Succeeded)
                return (false, pwResult.Errors.Select(e => e.Description).ToArray());
        }

        return (true, Array.Empty<string>());
    }

    public Task<(bool Succeeded, string[] Errors)> UpdateAdmin(SaveAdminViewModel vm) => UpdateUser(vm);
    public Task<(bool Succeeded, string[] Errors)> UpdateDeveloper(SaveDeveloperViewModel vm) => UpdateUser(vm);

    public async Task<(bool Succeeded, string[] Errors)> ToggleAdminStatus(string id, bool activate, string currentUserId)
    {
        if (id == currentUserId && !activate)
            return (false, new[] { "No puede inactivar a su propio usuario." });

        if (!activate)
        {
            var activeAdmins = await GetByRole(RoleAdmin);
            if (activeAdmins.Count(a => a.IsActive && a.Id != id) == 0)
                return (false, new[] { "Debe existir al menos un administrador activo en el sistema." });
        }

        return await SetActive(id, activate);
    }

    public Task<(bool Succeeded, string[] Errors)> ToggleDeveloperStatus(string id, bool activate) =>
        SetActive(id, activate);

    public Task<(bool Succeeded, string[] Errors)> ToggleAgentStatus(string id, bool activate) =>
        SetActive(id, activate);

    private async Task<(bool, string[])> SetActive(string id, bool activate)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return (false, new[] { "El usuario solicitado no existe." });

        user.IsActive = activate;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded
            ? (true, Array.Empty<string>())
            : (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Succeeded, string[] Errors)> DeleteAgent(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return (false, new[] { "El agente seleccionado no existe." });

        var agentProperties = await _propertyRepository.FindAsync(p => p.AgentId == id);

        foreach (var property in agentProperties)
        {
            await _propertyRepository.DeleteAsync(property);
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? (true, Array.Empty<string>())
            : (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<AdminDashboardViewModel> GetDashboardCounts()
    {
        var allProperties = await _propertyRepository.GetAllAsync();
        var agents = await _userManager.GetUsersInRoleAsync(RoleAgent);
        var clients = await _userManager.GetUsersInRoleAsync(RoleClient);
        var developers = await _userManager.GetUsersInRoleAsync(RoleDeveloper);

        return new AdminDashboardViewModel
        {
            AvailableProperties = allProperties.Count(p => p.Status == Core.Domain.Enums.PropertyStatus.Disponible),
            SoldProperties = allProperties.Count(p => p.Status == Core.Domain.Enums.PropertyStatus.Vendida),
            ActiveAgents = agents.Count(a => a.IsActive),
            InactiveAgents = agents.Count(a => !a.IsActive),
            ActiveClients = clients.Count(c => c.IsActive),
            InactiveClients = clients.Count(c => !c.IsActive),
            ActiveDevelopers = developers.Count(d => d.IsActive),
            InactiveDevelopers = developers.Count(d => !d.IsActive)
        };
    }
}