using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserDto?> FindByIdAsync(string userId);
    Task<UserDto?> FindByEmailAsync(string email);
    Task<UserDto?> FindByNameAsync(string username);
    Task<List<UserDto>> GetUsersInRoleAsync(string role);

    Task<(bool Succeeded, string Error, string? UserId)> CreateUserAsync(string userName, string email, string password, string firstName, string lastName, string? phoneNumber, bool isActive, string? profilePictureUrl);
    Task AddToRoleAsync(string userId, string role);
    Task<List<string>> GetRolesAsync(string userId);
    Task<bool> UpdateUserAsync(string userId, string firstName, string lastName, string email, string? phoneNumber);
    Task<bool> UpdateProfilePictureAsync(string userId, string url);
    Task SetActiveAsync(string userId, bool isActive);

    Task<bool> RoleExistsAsync(string role);
    Task CreateRoleAsync(string role);

    Task<(bool Succeeded, string Error)> PasswordSignInAsync(string userNameOrEmail, string password);
    Task SignOutAsync();
}
