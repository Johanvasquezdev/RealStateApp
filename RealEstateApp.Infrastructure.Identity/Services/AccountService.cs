using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.UsuarioOCorreo)
                   ?? await _userManager.FindByNameAsync(request.UsuarioOCorreo);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Los datos de acceso son inválidos o el usuario está inactivo.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Los datos de acceso son inválidos.");

        var roles = await _userManager.GetRolesAsync(user);
        
        if (roles.Contains("Cliente") || roles.Contains("Agente"))
        {
            throw new RealEstateApp.Core.Application.Exceptions.ForbiddenAccessException("El usuario no tiene permisos para acceder a la API.");
        }
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthenticateResponse
        {
            Token = tokenHandler.WriteToken(token),
            Usuario = user.UserName!,
            Roles = roles.ToList(),
            Expiracion = tokenDescriptor.Expires!.Value
        };
    }
}
