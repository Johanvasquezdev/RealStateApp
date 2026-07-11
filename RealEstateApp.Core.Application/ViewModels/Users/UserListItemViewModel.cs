namespace RealEstateApp.Core.Application.ViewModels.Users
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? IdCard { get; set; }
        public bool IsActive { get; set; }
        public int PropertiesCount { get; set; }
    }
}
