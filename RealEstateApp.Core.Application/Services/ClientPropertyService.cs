using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Properties;

namespace RealEstateApp.Core.Application.Services;

public class ClientPropertyService : Interfaces.Services.ClientPropertyService
{
    // Usaremos IPropiedadService de Gregori (que ya interactúa con la BD y el UnitOfWork)
    // para re-aprovechar la lógica de obtención de propiedades, 
    // pero mapearemos a nuestros Client ViewModels y buscaremos datos del agente.
    private readonly IPropertyService _propiedadService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ClientPropertyService(
        IPropertyService propiedadService,
        IMapper mapper,
        IUserService userService)
    {
        _propiedadService = propiedadService;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<List<ClientPropertyViewModel>> GetPropertiesWithFiltersAsync(ClientFilterPropertyViewModel filters)
    {
        // 1. Reutilizamos la lógica del agente para obtener las disponibles (o creamos una consulta propia en el repositorio si es necesario)
        // Por ahora, asumimos que Gregori tiene GetPropiedadesDisponiblesAsync, pero él devuelve AgentPropertyViewModel.
        // Lo ideal es tener acceso al repositorio genérico para mapear directamente a nuestro ClientPropertyViewModel.
        
        throw new NotImplementedException();
    }

    public async Task<ClientPropertyDetailViewModel?> GetPropertyDetailAsync(int id)
    {
        throw new NotImplementedException();
    }
}
