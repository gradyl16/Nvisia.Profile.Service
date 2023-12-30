namespace Nvisia.Profile.Service.Api.Models.Certification;

public class BatchCertificationRequest
{
    public int ProfileId { get; set; }
    
    public ICollection<CertificationRequest> Certifications { get; set; } = new List<CertificationRequest>();
}