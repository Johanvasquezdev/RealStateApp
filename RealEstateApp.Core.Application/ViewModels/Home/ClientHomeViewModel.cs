using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Core.Application.ViewModels.Home;

public class ClientHomeViewModel
{
    public List<ClientPropertyViewModel> Properties { get; set; } = new();
    public ClientFilterPropertyViewModel Filters { get; set; } = new();
    public string? SearchCode { get; set; }
}
