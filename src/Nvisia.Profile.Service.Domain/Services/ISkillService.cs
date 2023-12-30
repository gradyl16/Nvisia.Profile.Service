using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Domain.Services;

public interface ISkillService
{
    Task<ICollection<SkillDTO>> BatchSkills(int profileId, ICollection<SkillDTO> skills);
}