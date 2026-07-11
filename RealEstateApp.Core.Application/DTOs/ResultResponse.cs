namespace RealEstateApp.Core.Application.DTOs
{
    public class ResultResponse
    {
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; } = [];
    }
}
