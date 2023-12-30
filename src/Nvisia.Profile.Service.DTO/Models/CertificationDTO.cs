namespace Nvisia.Profile.Service.DTO.Models;

/// <summary>
/// A Certification.
/// This is the Data Transfer Object (DTO) used to communicate with the Api consumers. We do not want to expose the entity classes.
/// We are using the AutoMapper Package to map between DTO and Entity Objects. The member names should match between the two.
/// </summary>
public class CertificationDTO
{
    public int? CertificationId { get; set; }
    
    public string Title { get; set; } = null!;
    
    public int Year { get; set; }
    
    public int ProfileId { get; set; }
}