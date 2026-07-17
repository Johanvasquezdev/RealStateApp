using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Domain.Entities;

public class PunchCardRecord : AuditableBaseEntity
{
    public int PunchCardSessionId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime Date { get; set; }
    public string? ClockInTime { get; set; }
    public string? ClockOutTime { get; set; }
    public decimal? HoursWorked { get; set; }
    public PunchCardStatus Status { get; set; } = PunchCardStatus.Normal;
    public string? Notes { get; set; }
    public bool NameEdited { get; set; }
    public string? OriginalName { get; set; }

    public PunchCardSession PunchCardSession { get; set; } = null!;
}
