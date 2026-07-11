namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class PropertyTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int PropertiesCount { get; set; }
    }
}
