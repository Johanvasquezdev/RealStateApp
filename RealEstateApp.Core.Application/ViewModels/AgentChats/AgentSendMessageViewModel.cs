namespace RealEstateApp.Core.Application.ViewModels.AgentChats;

public class AgentSendMessageViewModel
{
    public int PropertyId { get; set; }
    public string ReceiverId { get; set; } = null!;
    public string Content { get; set; } = null!;
}
