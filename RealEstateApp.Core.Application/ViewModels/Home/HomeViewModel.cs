using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Core.Application.ViewModels.Home;

public class HomeViewModel
{
    public List<PropertyViewModel> Properties { get; set; } = new();
    public FilterPropertyViewModel Filters { get; set; } = new();
    public string? SearchCode { get; set; }
}
