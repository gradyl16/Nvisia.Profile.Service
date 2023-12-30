using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public interface IEducationRepository
{
    Task<ICollection<EducationEntity>> WriteEducations(ICollection<EducationEntity> educations);
}