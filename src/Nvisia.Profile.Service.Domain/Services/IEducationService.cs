using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Domain.Services;

public interface IEducationService
{
    Task<ICollection<EducationDTO>> BatchEducations(int profileId, ICollection<EducationDTO> educations);
}