using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Users;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class UserManagementService: IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<Property> _propertyRepository;
    
    private readonly string RoleAdmin = Roles.Administrador.ToString();
    private readonly string RoleDeveloper = Roles.Desarrollador.ToString();
    private readonly string RoleAgent = Roles.Agente.ToString();
    private readonly string RoleClient = Roles.Cliente.ToString();

    public UserManagementService(UserManager<ApplicationUser> userManager, IGenericRepository<Property> propertyRepository)
    {
        _userManager = userManager;
        _propertyRepository = propertyRepository;
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

    public Task<ResultResponse> CreateAdmin(SaveAdminViewModel vm) =>
        CreateUserWithRole(vm, RoleAdmin);

    public Task<ResultResponse> CreateDeveloper(SaveDeveloperViewModel vm)
    {
        var adminVm = new SaveAdminViewModel
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            IdCard = vm.IdCard,
            Email = vm.Email,
            UserName = vm.UserName,
            Password = vm.Password,
            ConfirmPassword = vm.ConfirmPassword
        };
        return CreateUserWithRole(adminVm, RoleDeveloper);
    }
    
    public Task<ResultResponse> UpdateAdmin(SaveAdminViewModel vm) => UpdateUser(vm);

    public Task<ResultResponse> UpdateDeveloper(SaveDeveloperViewModel vm)
    {
        var adminVm = new SaveAdminViewModel
        {
            Id = vm.Id,
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            IdCard = vm.IdCard,
            Email = vm.Email,
            UserName = vm.UserName,
            Password = vm.Password,
            ConfirmPassword = vm.ConfirmPassword
        };
        return UpdateUser(adminVm);
    }

    public async Task<ResultResponse> ToggleAdminStatus(string id, bool activate, string currentUserId)
    {
        if (id == currentUserId && !activate)
            return new ResultResponse { Succeeded = false, Errors = ["No puede inactivar a su propio usuario."] };

        if (!activate)
        {
            var activeAdmins = await GetByRole(RoleAdmin);
            if (activeAdmins.Count(a => a.IsActive && a.Id != id) == 0)
                return new ResultResponse { Succeeded = false, Errors = ["Debe existir al menos un administrador activo en el sistema."] };
        }

        return await SetActive(id, activate);
    }

    public Task<ResultResponse> ToggleDeveloperStatus(string id, bool activate) =>
        SetActive(id, activate);

    public Task<ResultResponse> ToggleAgentStatus(string id, bool activate) =>
        SetActive(id, activate);

    public async Task<ResultResponse> DeleteAgent(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return new ResultResponse { Succeeded = false, Errors = ["El agente seleccionado no existe."] };

        var agentProperties = await _propertyRepository.FindAsync(p => p.AgentId == id);

        foreach (var property in agentProperties)
        {
            await _propertyRepository.DeleteAsync(property);
        }

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded ? new ResultResponse { Succeeded = true } : new ResultResponse { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToArray() };
    }

    public async Task<AdminDashboardViewModel> GetDashboardCounts()
    {
        var allProperties = await _propertyRepository.GetAllAsync();
        var agents = await _userManager.GetUsersInRoleAsync(RoleAgent);
        var clients = await _userManager.GetUsersInRoleAsync(RoleClient);
        var developers = await _userManager.GetUsersInRoleAsync(RoleDeveloper);

        return new AdminDashboardViewModel
        {
            AvailableProperties = allProperties.Count(p => p.Status == PropertyStatus.Disponible),
            SoldProperties = allProperties.Count(p => p.Status == PropertyStatus.Vendida),
            ActiveAgents = agents.Count(a => a.IsActive),
            InactiveAgents = agents.Count(a => !a.IsActive),
            ActiveClients = clients.Count(c => c.IsActive),
            InactiveClients = clients.Count(c => !c.IsActive),
            ActiveDevelopers = developers.Count(d => d.IsActive),
            InactiveDevelopers = developers.Count(d => !d.IsActive)
        };
    }


    #region Private Helper Methods
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
    
    private async Task<ResultResponse> UpdateUser(SaveAdminViewModel vm)
    {
        var user = await _userManager.FindByIdAsync(vm.Id!);
        if (user is null)
            return new ResultResponse { Succeeded = false, Errors = ["El usuario solicitado no existe."] };

        user.FirstName = vm.FirstName;
        user.LastName = vm.LastName;
        user.IdCard = vm.IdCard;
        user.Email = vm.Email;
        user.UserName = vm.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return new ResultResponse { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToArray() };

        if (!string.IsNullOrWhiteSpace(vm.Password))
        {
            if (vm.Password != vm.ConfirmPassword)
                return new ResultResponse { Succeeded = false, Errors = ["La contraseña y la confirmación de contraseña no coinciden."] };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var pwResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
            if (!pwResult.Succeeded)
                return new ResultResponse { Succeeded = false, Errors = pwResult.Errors.Select(e => e.Description).ToArray() };
        }

        return new ResultResponse { Succeeded = true };
    }

    private async Task<ResultResponse> SetActive(string id, bool activate)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return new ResultResponse { Succeeded = false, Errors = ["El usuario solicitado no existe."] };

        user.IsActive = activate;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded ? new ResultResponse { Succeeded = true } : 
            new ResultResponse { Succeeded = false, Errors = [.. result.Errors.Select(e => e.Description)] };
    }

    private async Task<ResultResponse> CreateUserWithRole(SaveAdminViewModel vm, string role)
    {
        if (vm.Password != vm.ConfirmPassword)
            return new ResultResponse { Succeeded = false, Errors = ["las claves no coinciden."] };

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
            return new ResultResponse { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToArray() };

        await _userManager.AddToRoleAsync(user, role);
        return new ResultResponse { Succeeded = true };
    }
    #endregion
}