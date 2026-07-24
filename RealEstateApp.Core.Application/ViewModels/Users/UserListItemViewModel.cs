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
        
        public string ObfuscatedIdCard 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(IdCard)) return string.Empty;
                var clean = new string(IdCard.Where(char.IsDigit).ToArray());
                if (clean.Length == 11)
                    return $"{clean.Substring(0, 3)}-*******-{clean.Substring(10, 1)}";
                if (clean.Length >= 4)
                    return new string('*', clean.Length - 4) + clean.Substring(clean.Length - 4);
                return clean;
            }
        }

        public bool IsActive { get; set; }
        public int PropertiesCount { get; set; }
    }
}
