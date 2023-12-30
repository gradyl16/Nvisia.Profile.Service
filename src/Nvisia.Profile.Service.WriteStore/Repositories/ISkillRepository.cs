using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public interface ISkillRepository
{
    Task<ICollection<SkillEntity>> WriteSkills(ICollection<SkillEntity> skills);
}