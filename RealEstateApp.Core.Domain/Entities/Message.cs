using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class Message : AuditableBaseEntity
{
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public string Content { get; set; } = null!;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
