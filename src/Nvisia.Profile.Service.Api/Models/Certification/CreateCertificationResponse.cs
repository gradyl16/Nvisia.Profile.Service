namespace Nvisia.Profile.Service.Api.Models.Certification;

public class CreateCertificationResponse
{
    public int CertificationId { get; set; }
    
    public string Title { get; set; } = null!;
    
    public int Year { get; set; }
}