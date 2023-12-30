using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public interface ISkillCodeRepository
{
    Task<ICollection<SkillCodeEntity>> GetSkillCodes();
}