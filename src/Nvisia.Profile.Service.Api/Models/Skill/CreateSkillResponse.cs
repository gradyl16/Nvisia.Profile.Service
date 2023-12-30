namespace Nvisia.Profile.Service.Api.Models.Skill;

public class CreateSkillResponse
{
    public int SkillId { get; set; }
    
    public string Description { get; set; } = null!;

    public int SkillCodeId { get; set; } 

    public string SkillCodeCode { get; set; } = null!;

    public string SkillCodeLabel { get; set; } = null!;
}