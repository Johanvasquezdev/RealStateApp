using RealEstateApp.Core.Application.DTOs.PunchCard;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ImporterPunchCardService
{
    Task<List<RowPunchCardImportada>> LeerArchivoAsync(Stream stream, string nombreArchivo);
}
