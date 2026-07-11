namespace RealEstateApp.Core.Application.ViewModels.Agente;

public class AgenteListViewModel
{
    public string Id { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string NombreCompleto => $"{FirstName} {LastName}";
    public int CantidadPropiedades { get; set; }
    public string Email { get; set; } = null!;
}
