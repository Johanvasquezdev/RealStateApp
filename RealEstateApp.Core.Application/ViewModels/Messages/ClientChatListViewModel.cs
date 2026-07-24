namespace RealEstateApp.Core.Application.ViewModels.Messages;

public class ClientChatListViewModel
{
    public string AgentId { get; set; } = null!;
    public string AgentName { get; set; } = null!;
    public string? AgentPhotoUrl { get; set; }
    public string LastMessage { get; set; } = null!;
    public DateTime LastMessageDate { get; set; }
    public int PropertyId { get; set; }
}
