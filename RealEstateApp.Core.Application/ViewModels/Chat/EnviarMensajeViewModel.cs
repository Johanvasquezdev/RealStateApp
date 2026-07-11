namespace RealEstateApp.Core.Application.ViewModels.Chat;

public class EnviarMensajeViewModel
{
    public int PropertyId { get; set; }
    public string ReceiverId { get; set; } = null!;
    public string Content { get; set; } = null!;
}
