namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ExportPunchCardService
{
    Task<byte[]> ExportExcelAsync(int sessionId);
    Task<byte[]> ExportPdfAsync(int sessionId);
    Task<byte[]> ExportWordAsync(int sessionId);
}
