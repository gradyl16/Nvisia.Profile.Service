namespace Nvisia.Profile.Service.Api.Models.SkillCode;

public class SkillCodeResponse
{
    public int SkillCodeId { get; set; }
    
    public string Code { get; set; } = null!;
    
    public string Label { get; set; } = null!;
}