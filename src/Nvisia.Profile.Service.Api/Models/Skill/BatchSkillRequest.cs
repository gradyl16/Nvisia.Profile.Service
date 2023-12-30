namespace Nvisia.Profile.Service.Api.Models.Skill;

public class BatchSkillRequest
{
    public int ProfileId { get; set; }
    
    public ICollection<SkillRequest> Skills { get; set; } = new List<SkillRequest>();
}