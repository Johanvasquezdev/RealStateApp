using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.DTOs;
using RealEstateApp.Core.Application.Interfaces;
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
    private readonly IAccountService _accountService;

    public AccountController(UserManager<ApplicationUser> userManager, IAccountService accountService)
    {
        _userManager = userManager;
        _accountService = accountService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        try
        {
            var response = await _accountService.AuthenticateAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
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
