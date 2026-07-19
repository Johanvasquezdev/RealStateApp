using RealEstateApp.Core.Application.DTOs;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IAccountService
{
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request);
}
