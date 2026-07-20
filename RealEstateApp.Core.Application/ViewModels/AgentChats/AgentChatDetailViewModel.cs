namespace RealEstateApp.Core.Application.ViewModels.AgentChats;

public class AgentChatDetailViewModel
{
    public string ClientId { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string? ClientPhoto { get; set; }
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = null!;
    public List<MessageViewModel> Messages { get; set; } = new();
}

public class MessageViewModel
{
    public string Content { get; set; } = null!;
    public string SenderId { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public DateTime Created { get; set; }
}
