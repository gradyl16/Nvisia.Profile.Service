namespace Nvisia.Profile.Service.DTO.Models;

/// <summary>
/// The Profile and the children of the Profile.
/// This is the Data Transfer Object (DTO) used to communicate with the Api consumers. We do not want to expose the entity classes.
/// We are using the AutoMapper Package to map between DTO and Entity Objects. The member names should match between the two.
/// </summary>
public class ProfileDTO
{
    public int? ProfileId { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public  string EmailAddress { get; set; } = null!;
    
    public int TitleCodeId { get; set; }
    
    public int YearsOfExperience { get; set; }
    
    public string? AboutMe { get; set; }
    
    public TitleCodeDTO TitleCode { get; set; } = null!;

    public ICollection<CertificationDTO> Certifications { get; set; } = new List<CertificationDTO>();
    
    public ICollection<EducationDTO> Educations { get; set; } = new List<EducationDTO>();
    
    public ICollection<HighlightDTO> Highlights { get; set; } = new List<HighlightDTO>();
    
    public ICollection<SkillDTO> Skills { get; set; } = new List<SkillDTO>();
}