namespace RealEstateApp.Core.Application.ViewModels.AgentChats;

public class AgentChatDetailViewModel
{
    public string ClienteId { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = null!;
    public List<MensajeViewModel> Mensajes { get; set; } = new();
}

public class MensajeViewModel
{
    public string Content { get; set; } = null!;
    public string SenderId { get; set; } = null!;
    public string SenderNombre { get; set; } = null!;
    public DateTime Created { get; set; }
}
