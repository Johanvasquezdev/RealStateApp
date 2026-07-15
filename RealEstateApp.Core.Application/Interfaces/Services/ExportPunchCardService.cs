namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ExportPunchCardService
{
    Task<byte[]> ExportarExcelAsync(int sesionId);
    Task<byte[]> ExportarPdfAsync(int sesionId);
    Task<byte[]> ExportarWordAsync(int sesionId);
}
