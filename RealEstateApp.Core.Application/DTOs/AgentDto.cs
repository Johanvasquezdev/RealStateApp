namespace RealEstateApp.Core.Application.DTOs;

public class AgentDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public int PropertiesCount { get; set; }
}
