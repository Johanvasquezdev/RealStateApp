using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class ImprovementService(IGenericRepository<Improvement> repository, IMapper mapper)
    : GenericService<SaveImprovementViewModel, ImprovementViewModel, Improvement>(repository, mapper), IImprovementService
    {
    }
}
