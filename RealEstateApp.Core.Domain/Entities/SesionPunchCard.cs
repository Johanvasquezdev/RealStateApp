using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class PunchCardSession : AuditableBaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int ErrorRecords { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public ICollection<PunchCardRecord> Records { get; set; } = new List<PunchCardRecord>();
}
