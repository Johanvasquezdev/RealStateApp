using RealEstateApp.Core.Application.DTOs.PunchCard;

namespace RealEstateApp.Core.Application.Interfaces;

public interface IImporterPunchCardService
{
    Task<List<ImportedPunchCardRow>> ReadFileAsync(Stream stream, string fileName);
}



