using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public interface ICertificationRepository
{
    Task<ICollection<CertificationEntity>> WriteCertifications(ICollection<CertificationEntity> certifications);
    Task<bool> DeleteCertificationsByProfileId(int profileId);
}