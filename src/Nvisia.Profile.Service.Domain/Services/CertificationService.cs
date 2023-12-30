using AutoMapper;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain.Services;

public class CertificationService : ICertificationService
{
    private readonly ICertificationRepository _certificationRepository;

    private readonly IMapper _mapper;

    public CertificationService(ICertificationRepository certificationRepository, IMapper mapper)
    {
        _certificationRepository =
            certificationRepository ?? throw new ArgumentNullException(nameof(certificationRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ICollection<CertificationDTO>?> BatchCertifications(int profileId,
        ICollection<CertificationDTO> certifications)
    {
        if (certifications.Count == 0)
        {
            var deleted = await _certificationRepository.DeleteCertificationsByProfileId(profileId);
            return deleted
                ? new List<CertificationDTO>()
                : null;
        }

        var certificationEntities = _mapper.Map<ICollection<CertificationEntity>>(certifications);
        certificationEntities.ToList().ForEach(x => x.ProfileId = profileId);
        var results = await _certificationRepository.WriteCertifications(certificationEntities);
        var certificationDTOs = _mapper.Map<ICollection<CertificationDTO>>(results);
        return certificationDTOs;
    }
}