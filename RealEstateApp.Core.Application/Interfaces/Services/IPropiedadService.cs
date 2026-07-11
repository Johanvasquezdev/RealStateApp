using RealEstateApp.Core.Application.ViewModels.Propiedad;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IPropiedadService
{
    Task<List<PropiedadViewModel>> GetPropiedadesDisponiblesAsync();
    Task<List<PropiedadViewModel>> GetPropiedadesByAgenteAsync(string agenteId);
    Task<List<PropiedadViewModel>> GetPropiedadesDisponiblesByAgenteAsync(string agenteId);
    Task<PropiedadViewModel?> GetPropiedadDetailAsync(int id);
    Task<PropiedadSaveViewModel> GetPropiedadForEditAsync(int id, string agenteId);
    Task<int> CreateAsync(PropiedadSaveViewModel vm, string agenteId);
    Task UpdateAsync(PropiedadSaveViewModel vm, string agenteId);
    Task DeleteAsync(int id, string agenteId);
    Task<string?> GetFirstImageAsync(int propertyId);
}
