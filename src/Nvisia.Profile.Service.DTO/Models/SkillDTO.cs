namespace Nvisia.Profile.Service.DTO.Models;

/// <summary>
/// A Skill.
/// This is the Data Transfer Object (DTO) used to communicate with the Api consumers. We do not want to expose the entity classes.
/// We are using the AutoMapper Package to map between DTO and Entity Objects. The member names should match between the two.
/// </summary>
public class SkillDTO
{
    public int? SkillId { get; set; }

    public string Description { get; set; } = null!;

    public int SkillCodeId { get; set; }

    public int ProfileId { get; set; }

    public SkillCodeDTO SkillCode { get; set; } = null!;
}