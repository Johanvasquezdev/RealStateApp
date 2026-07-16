namespace RealEstateApp.Core.Application.ViewModels.Messages;

public class ClientChatConversationViewModel
{
    public string AgentId { get; set; } = null!;
    public string AgentName { get; set; } = null!;
    public string? AgentTitle { get; set; }
    public string? AgentPhotoUrl { get; set; }
    public int AgentPropertyCount { get; set; }
    public int PropertyId { get; set; }
    public List<ChatBubbleViewModel> Messages { get; set; } = new();
}

public class ChatBubbleViewModel
{
    public string Content { get; set; } = null!;
    public bool IsFromClient { get; set; }
    public DateTime SentAt { get; set; }
}
