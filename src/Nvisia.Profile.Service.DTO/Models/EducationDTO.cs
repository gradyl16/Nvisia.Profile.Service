namespace Nvisia.Profile.Service.DTO.Models;

/// <summary>
/// An Education entry.
/// This is the Data Transfer Object (DTO) used to communicate with the Api consumers. We do not want to expose the entity classes.
/// We are using the AutoMapper Package to map between DTO and Entity Objects. The member names should match between the two.
/// </summary>
public class EducationDTO
{
    public int? EducationId { get; set; }

    public string SchoolName { get; set; } = null!;
    
    public int GraduationYear { get; set; }

    public string? MajorDegreeName { get; set; }
    
    public string? MinorDegreeName { get; set; } 
    
    public int ProfileId { get; set; }

}