using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    private string _idCard = null!;
    public string IdCard
    {
        get => _idCard;
        set => _idCard = string.IsNullOrEmpty(value) ? string.Empty : new string(value.Where(char.IsDigit).ToArray());
    }
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
