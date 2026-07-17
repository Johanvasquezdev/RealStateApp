namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class ImportedPunchCardRow
{
    public string EmployeeName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime Date { get; set; }
    public string? ClockInTime { get; set; }
    public string? ClockOutTime { get; set; }
    public decimal? HoursWorked { get; set; }
}
