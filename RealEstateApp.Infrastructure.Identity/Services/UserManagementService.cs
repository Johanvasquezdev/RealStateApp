using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Users;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class UserManagementService: IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGenericRepository<Property> _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    
    private readonly string RoleAdmin = Roles.Administrador.ToString();
    private readonly string RoleDeveloper = Roles.Desarrollador.ToString();
    private readonly string RoleAgent = Roles.Agente.ToString();
    private readonly string RoleClient = Roles.Cliente.ToString();

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        IGenericRepository<Property> propertyRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _userManager = userManager;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
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

        var agentProperties = await _propertyRepository.FindWithIncludesAsync(p => p.AgentId == id, "Images");
        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var property in agentProperties)
                await _propertyRepository.DeleteAsync(property);

            await _unitOfWork.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                return new ResultResponse { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToArray() };
            }

            await transaction.CommitAsync();

            foreach (var property in agentProperties)
            foreach (var image in property.Images)
                _fileStorageService.DeleteFile(image.ImageUrl, "properties");

            return new ResultResponse { Succeeded = true };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return new ResultResponse { Succeeded = false, Errors = [ex.Message] };
        }
    }

    public async Task<AdminDashboardViewModel> GetDashboardCounts()
    {
        var allProperties = await _propertyRepository.GetAllAsync();
        var agents = await _userManager.GetUsersInRoleAsync(RoleAgent);
        var clients = await _userManager.GetUsersInRoleAsync(RoleClient);
        var developers = await _userManager.GetUsersInRoleAsync(RoleDeveloper);

        return new AdminDashboardViewModel
        {
            AvailableProperties = allProperties.Count(p => p.Status == PropertyStatus.Available),
            SoldProperties = allProperties.Count(p => p.Status == PropertyStatus.Sold),
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
            IdCard = u.IdCard,
            Email = u.Email!,
            UserName = u.UserName!,
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
        var cleanIdCard = NormalizeIdCard(vm.IdCard);
        if (_userManager.Users.Any(u => u.IdCard == cleanIdCard && u.Id != user.Id))
            return new ResultResponse { Succeeded = false, Errors = ["Ya existe un usuario registrado con esta cédula."] };

        var existingUserName = await _userManager.FindByNameAsync(vm.UserName);
        if (existingUserName is not null && existingUserName.Id != user.Id)
            return new ResultResponse { Succeeded = false, Errors = ["Ya existe un usuario registrado con este nombre de usuario."] };

        var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
        if (existingEmail is not null && existingEmail.Id != user.Id)
            return new ResultResponse { Succeeded = false, Errors = ["Ya existe un usuario registrado con este correo electrónico."] };

        user.IdCard = cleanIdCard;
        user.UserName = vm.UserName.Trim();
        user.NormalizedUserName = _userManager.NormalizeName(user.UserName);
        user.Email = vm.Email.Trim();
        user.NormalizedEmail = _userManager.NormalizeEmail(user.Email);

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
        if (!result.Succeeded)
            return new ResultResponse { Succeeded = false, Errors = [.. result.Errors.Select(e => e.Description)] };

        if (!activate)
        {
            var stampResult = await _userManager.UpdateSecurityStampAsync(user);
            if (!stampResult.Succeeded)
                return new ResultResponse { Succeeded = false, Errors = [.. stampResult.Errors.Select(e => e.Description)] };
        }

        return new ResultResponse { Succeeded = true };
    }

    private async Task<ResultResponse> CreateUserWithRole(SaveAdminViewModel vm, string role)
    {
        if (string.IsNullOrWhiteSpace(vm.Password) || string.IsNullOrWhiteSpace(vm.ConfirmPassword))
            return new ResultResponse { Succeeded = false, Errors = ["La contraseña es obligatoria al crear el usuario."] };

        if (vm.Password != vm.ConfirmPassword)
            return new ResultResponse { Succeeded = false, Errors = ["las claves no coinciden."] };

        string cleanIdCard = NormalizeIdCard(vm.IdCard);
        if (_userManager.Users.Any(u => u.IdCard == cleanIdCard))
            return new ResultResponse { Succeeded = false, Errors = ["Ya existe un usuario registrado con esta cédula."] };

        var user = new ApplicationUser
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            IdCard = cleanIdCard,
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

    private static string NormalizeIdCard(string? idCard) =>
        new string((idCard ?? string.Empty).Where(char.IsDigit).ToArray());
    #endregion
}
