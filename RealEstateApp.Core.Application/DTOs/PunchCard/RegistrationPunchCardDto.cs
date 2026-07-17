using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.DTOs.PunchCard;

public class RegistrationPunchCardDto
{
    public int Id { get; set; }
    public int PunchCardSessionId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime Date { get; set; }
    public string? ClockInTime { get; set; }
    public string? ClockOutTime { get; set; }
    public decimal? HoursWorked { get; set; }
    public PunchCardStatus Status { get; set; }
    public string? Notes { get; set; }
    public bool NameEdited { get; set; }
}
