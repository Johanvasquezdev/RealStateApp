using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class ImprovementService(IGenericRepository<Improvement> repository, IMapper mapper)
    : GenericService<SaveImprovementViewModel, ImprovementViewModel, Improvement>(repository, mapper), Interfaces.ImprovementService
    {
        private readonly IGenericRepository<PropertyImprovement> _linkRepository = linkRepository;
        public override async Task Delete(int id)
        {
            var improvement = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("La mejora solicitada no existe.");

            var affectedLinks = await _linkRepository.FindAsync(l => l.ImprovementId == id);
            foreach (var link in affectedLinks)
            {
                await _linkRepository.DeleteAsync(link);
            }

            await _repository.DeleteAsync(improvement);
        }
    }
}
