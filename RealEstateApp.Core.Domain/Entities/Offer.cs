using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Domain.Entities;

public class Offer : AuditableBaseEntity
{
    public string ClientId { get; set; } = null!; // Relates to Identity User
    public double Amount { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
