using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Domain.Services;

public interface ICertificationService
{
    Task<ICollection<CertificationDTO>?> BatchCertifications(int profileId, ICollection<CertificationDTO> certifications);
}