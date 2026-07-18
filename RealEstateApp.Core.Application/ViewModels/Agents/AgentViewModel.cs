namespace RealEstateApp.Core.Application.ViewModels.Agents;

public class AgentListViewModel
{
    public string Id { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string FullName => $"{FirstName} {LastName}";
    public int PropertyCount { get; set; }
    public string Email { get; set; } = null!;
    public bool Activate { get; set; }
}
