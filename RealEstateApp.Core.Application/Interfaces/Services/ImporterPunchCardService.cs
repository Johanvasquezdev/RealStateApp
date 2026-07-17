using RealEstateApp.Core.Application.DTOs.PunchCard;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ImporterPunchCardService
{
    Task<List<ImportedPunchCardRow>> ReadFileAsync(Stream stream, string fileName);
}
