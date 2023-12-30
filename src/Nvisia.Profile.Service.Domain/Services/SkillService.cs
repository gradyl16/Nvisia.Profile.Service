using AutoMapper;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain.Services;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;
    private readonly IMapper _mapper;

    public SkillService(ISkillRepository skillRepository, IMapper mapper)
    {
        _skillRepository = skillRepository ?? throw new ArgumentNullException(nameof(skillRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<ICollection<SkillDTO>> BatchSkills(int profileId, ICollection<SkillDTO> skills)
    {
        var skillEntities = _mapper.Map<ICollection<SkillEntity>>(skills)
            .Select((skill, index) =>
            {
                skill.ProfileId = profileId;
                skill.SortOrder = index;
                return skill;
            })
            .ToList();
        var results = await _skillRepository.WriteSkills(skillEntities);
        var skillDTOs = _mapper.Map<ICollection<SkillDTO>>(results);
        return skillDTOs;
    }
}