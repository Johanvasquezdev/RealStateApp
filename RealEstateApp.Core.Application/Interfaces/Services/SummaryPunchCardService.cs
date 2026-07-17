using RealEstateApp.Core.Application.DTOs.PunchCard;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ISummaryPunchCardService
{
    Task<PunchCardImportResultDto> ProcessImportAsync(List<ImportedPunchCardRow> rows, string fileName, string? userId, string? userName);
    Task<IEnumerable<SessionPunchCardDto>> GetSessionsAsync();
    Task<List<RegistrationPunchCardDto>> GetRecordsAsync(FilterPunchCardDto filter);
    Task<int> GetTotalRecordsAsync(FilterPunchCardDto filter);
    Task<RegistrationPunchCardDto?> GetByIdAsync(int id);
    Task<RegistrationPunchCardDto?> EditRecordAsync(EditPunchCardDto dto, string? editedBy);
}
