namespace RealEstateApp.Core.Application.ViewModels.Agents;

public class ClientAgentViewModel
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}
