using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string IdCard { get; set; } = null!; // Cedula
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
