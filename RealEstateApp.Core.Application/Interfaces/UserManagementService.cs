using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.ViewModels.Users;

namespace RealEstateApp.Core.Application.Interfaces;
public interface UserManagementService
{
    Task<List<UserListItemViewModel>> GetAllAdmins();
    Task<ResultResponse> CreateAdmin(SaveAdminViewModel vm);
    Task<ResultResponse> UpdateAdmin(SaveAdminViewModel vm);
    Task<ResultResponse> ToggleAdminStatus(string id, bool activate, string currentUserId);

    Task<List<UserListItemViewModel>> GetAllDevelopers();
    Task<ResultResponse> CreateDeveloper(SaveDeveloperViewModel vm);
    Task<ResultResponse> UpdateDeveloper(SaveDeveloperViewModel vm);
    Task<ResultResponse> ToggleDeveloperStatus(string id, bool activate);

    Task<List<UserListItemViewModel>> GetAllAgents();
    Task<ResultResponse> ToggleAgentStatus(string id, bool activate);
    Task<ResultResponse> DeleteAgent(string id);

    Task<AdminDashboardViewModel> GetDashboardCounts();
}