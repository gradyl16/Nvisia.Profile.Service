using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Domain.Services;

public interface ISkillCodeService
{
    Task<ICollection<SkillCodeDTO>> GetSkillCodes();
}