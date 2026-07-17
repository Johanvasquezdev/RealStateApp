namespace RealEstateApp.Core.Application.Interfaces;

public interface IExportPunchCardService
{
    Task<byte[]> ExportExcelAsync(int sessionId);
    Task<byte[]> ExportPdfAsync(int sessionId);
    Task<byte[]> ExportWordAsync(int sessionId);
}


