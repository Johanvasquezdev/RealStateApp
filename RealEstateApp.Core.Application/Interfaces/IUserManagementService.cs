using RealEstateApp.Core.Application.ViewModels.Users;

namespace RealEstateApp.Core.Application.Interfaces;
public interface IUserManagementService
{
    Task<List<UserListItemViewModel>> GetAllAdmins();
    Task<(bool Succeeded, string[] Errors)> CreateAdmin(SaveAdminViewModel vm);
    Task<(bool Succeeded, string[] Errors)> UpdateAdmin(SaveAdminViewModel vm);
    Task<(bool Succeeded, string[] Errors)> ToggleAdminStatus(string id, bool activate, string currentUserId);

    Task<List<UserListItemViewModel>> GetAllDevelopers();
    Task<(bool Succeeded, string[] Errors)> CreateDeveloper(SaveDeveloperViewModel vm);
    Task<(bool Succeeded, string[] Errors)> UpdateDeveloper(SaveDeveloperViewModel vm);
    Task<(bool Succeeded, string[] Errors)> ToggleDeveloperStatus(string id, bool activate);

    Task<List<UserListItemViewModel>> GetAllAgents();
    Task<(bool Succeeded, string[] Errors)> ToggleAgentStatus(string id, bool activate);
    Task<(bool Succeeded, string[] Errors)> DeleteAgent(string id);

    Task<AdminDashboardViewModel> GetDashboardCounts();
}