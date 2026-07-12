namespace RealEstateApp.Core.Application.ViewModels.AgentChats;

public class AgentChatSummaryViewModel
{
    public string ClienteId { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public string? UltimoMensaje { get; set; }
    public DateTime FechaUltimoMensaje { get; set; }
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = null!;
}
