using AutoMapper;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain.Services;

public class SkillCodeService : ISkillCodeService
{
    private readonly ISkillCodeRepository _skillCodeRepository;
    private readonly IMapper _mapper;

    public SkillCodeService(ISkillCodeRepository skillCodeRepository, IMapper mapper)
    {
        _skillCodeRepository = skillCodeRepository ?? throw new ArgumentNullException(nameof(skillCodeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ICollection<SkillCodeDTO>> GetSkillCodes()
    {
        var skillCodes = await _skillCodeRepository.GetSkillCodes();
        return _mapper.Map<ICollection<SkillCodeDTO>>(skillCodes);
    }
}