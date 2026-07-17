namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class SessionPunchCardDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int ErrorRecords { get; set; }
    public string? UserName { get; set; }
    public DateTime UploadDate { get; set; }
}
