using AutoMapper;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain.Services;

public class EducationService : IEducationService
{
    private readonly IEducationRepository _educationRepository;

    private readonly IMapper _mapper;

    public EducationService(IEducationRepository educationRepository, IMapper mapper)
    {
        _educationRepository = educationRepository ?? throw new ArgumentNullException(nameof(educationRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ICollection<EducationDTO>> BatchEducations(int profileId, ICollection<EducationDTO> educations)
    {
        var educationEntities = _mapper.Map<ICollection<EducationEntity>>(educations);
        educationEntities.ToList().ForEach(x => x.ProfileId = profileId);
        var results = await _educationRepository.WriteEducations(educationEntities);
        var educationDTOs = _mapper.Map<ICollection<EducationDTO>>(results);
        return educationDTOs;
    }
}