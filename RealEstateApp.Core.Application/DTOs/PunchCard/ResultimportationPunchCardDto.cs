namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class PunchCardImportResultDto
{
    public int SessionId { get; set; }
    public int TotalRows { get; set; }
    public int RecordsInserted { get; set; }
    public int DuplicatesSkipped { get; set; }
    public string FileName { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}
