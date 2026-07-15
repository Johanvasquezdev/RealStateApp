using RealEstateApp.Core.Application.DTOs.PunchCard;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface SummaryPunchCardService
{
    Task<ResultimportationPunchCardDto> ProcesarImportacionAsync(List<RowPunchCardImportada> filas, string nombreArchivo, string? usuarioId, string? usuarioNombre);
    Task<IEnumerable<SessionPunchCardDto>> GetSesionesAsync();
    Task<List<RegistrationPunchCardDto>> GetRegistrosAsync(FilterPunchCardDto filtro);
    Task<int> GetTotalRegistrosAsync(FilterPunchCardDto filtro);
    Task<RegistrationPunchCardDto?> GetByIdAsync(int id);
    Task<RegistrationPunchCardDto?> EditarRegistroAsync(EditPunchCardDto dto, string? editadoPor);
}
