namespace RealEstateApp.Core.Application.ViewModels.AgentChats;

public class AgentChatSummaryViewModel
{
    public string ClientId { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string? LastMessage { get; set; }
    public DateTime LastMessageDate { get; set; }
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = null!;
}
