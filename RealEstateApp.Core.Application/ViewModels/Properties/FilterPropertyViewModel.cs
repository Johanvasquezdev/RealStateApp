namespace RealEstateApp.Core.Application.ViewModels.Properties;

public class FilterPropertyViewModel
{
    public int? PropertyTypeId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? Rooms { get; set; }
    public int? Bathrooms { get; set; }
}
