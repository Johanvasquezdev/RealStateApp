using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.ViewModels.Users;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.UsuarioOCorreo)
                   ?? await _userManager.FindByNameAsync(request.UsuarioOCorreo);

        if (user is null || !user.IsActive)
            return Unauthorized(new { error = "Los datos de acceso son inválidos." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { error = "Los datos de acceso son inválidos." });

        var roles = await _userManager.GetRolesAsync(user);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:DurationInMinutes"] ?? "60")),
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new AuthenticateResponse
        {
            Token = tokenHandler.WriteToken(token),
            Usuario = user.UserName!,
            Roles = roles.ToList(),
            Expiracion = tokenDescriptor.Expires!.Value
        });
    }

    [HttpPost("register-developer")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegisterDeveloper([FromBody] SaveDeveloperViewModel vm)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = ModelState });

        if (vm.Password != vm.ConfirmPassword)
            return BadRequest(new { error = "La contraseña y la confirmación de contraseña no coinciden." });

        var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
        if (existingEmail is not null)
            return BadRequest(new { error = "Ya existe un usuario registrado con este correo electrónico." });

        var existingUserName = await _userManager.FindByNameAsync(vm.UserName);
        if (existingUserName is not null)
            return BadRequest(new { error = "Ya existe un usuario registrado con este nombre de usuario." });

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
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "Desarrollador");

        return StatusCode(201, new { message = "El desarrollador fue creado correctamente.", userId = user.Id });
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegisterAdmin([FromBody] SaveAdminViewModel vm)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = ModelState });

        if (vm.Password != vm.ConfirmPassword)
            return BadRequest(new { error = "La contraseña y la confirmación de contraseña no coinciden." });

        var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
        if (existingEmail is not null)
            return BadRequest(new { error = "Ya existe un usuario registrado con este correo electrónico." });

        var existingUserName = await _userManager.FindByNameAsync(vm.UserName);
        if (existingUserName is not null)
            return BadRequest(new { error = "Ya existe un usuario registrado con este nombre de usuario." });

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
            return BadRequest(new { error = "Los datos enviados no son válidos.", details = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, "Administrador");

        return StatusCode(201, new { message = "El administrador fue creado correctamente.", userId = user.Id });
    }
}
