using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<UserDto?> FindByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto?> FindByNameAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto?> FindByIdCardAsync(string idCard)
    {
        var normalizedIdCard = NormalizeIdCard(idCard);
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.IdCard == normalizedIdCard);
        return user is null ? null : ToDto(user);
    }

    public async Task<List<UserDto>> GetUsersInRoleAsync(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        return users.Select(ToDto).ToList();
    }

    public async Task<(bool Succeeded, string Error, string? UserId)> CreateUserAsync(string userName, string email, string password, string firstName, string lastName, string? phoneNumber, bool isActive, string? profilePictureUrl, string idCard)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IdCard = idCard,
            PhoneNumber = phoneNumber,
            IsActive = isActive,
            ProfilePictureUrl = profilePictureUrl ?? "default-profile.png"
        };

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded
            ? (true, string.Empty, user.Id)
            : (false, string.Join(" ", result.Errors.Select(e => e.Description)), null);
    }

    public async Task AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is not null)
            await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<List<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? new() : (await _userManager.GetRolesAsync(user)).ToList();
    }

    public async Task<bool> UpdateUserAsync(string userId, string firstName, string lastName, string email, string? phoneNumber)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
        user.PhoneNumber = phoneNumber;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UpdateProfilePictureAsync(string userId, string url)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;

        user.ProfilePictureUrl = url;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task SetActiveAsync(string userId, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return;

        user.IsActive = isActive;
        await _userManager.UpdateAsync(user);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return string.Empty;

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<(bool Succeeded, string Error)> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return (false, "User not found");

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded
            ? (true, string.Empty)
            : (false, string.Join(" ", result.Errors.Select(e => e.Description)));
    }

    public async Task<bool> RoleExistsAsync(string role)
    {
        return await _roleManager.RoleExistsAsync(role);
    }

    public async Task CreateRoleAsync(string role)
    {
        await _roleManager.CreateAsync(new IdentityRole(role));
    }

    public async Task<(bool Succeeded, string Error)> PasswordSignInAsync(string userNameOrEmail, string password)
    {
        var user = await _userManager.FindByEmailAsync(userNameOrEmail)
                   ?? await _userManager.FindByNameAsync(userNameOrEmail);

        if (user is null)
            return (false, "Los datos de acceso son inválidos.");

        if (!user.IsActive)
            return (false, "El usuario se encuentra inactivo y no puede iniciar sesión.");

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
        return result.Succeeded
            ? (true, string.Empty)
            : (false, "Los datos de acceso son inválidos.");
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    private static UserDto ToDto(ApplicationUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IdCard = user.IdCard,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    private static string NormalizeIdCard(string? idCard) =>
        new string((idCard ?? string.Empty).Where(char.IsDigit).ToArray());
}

