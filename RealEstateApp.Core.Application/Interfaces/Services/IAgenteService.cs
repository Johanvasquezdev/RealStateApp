using RealEstateApp.Core.Application.ViewModels.Agente;
using RealEstateApp.Core.Application.ViewModels.Propiedad;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IAgenteService
{
    Task<List<AgenteListViewModel>> GetAgentesActivosAsync(string? nombreFilter);
    Task<AgenteListViewModel?> GetAgenteByIdAsync(string id);
    Task<List<PropiedadViewModel>> GetPropiedadesByAgenteAsync(string id);
}
