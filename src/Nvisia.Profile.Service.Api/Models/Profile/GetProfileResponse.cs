using Nvisia.Profile.Service.Api.Models.Certification;
using Nvisia.Profile.Service.Api.Models.Education;
using Nvisia.Profile.Service.Api.Models.Highlight;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Api.Models.TitleCode;

namespace Nvisia.Profile.Service.Api.Models.Profile;

public class GetProfileResponse
{
    public int ProfileId { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public  string EmailAddress { get; set; } = null!;
    
    public int YearsOfExperience { get; set; }
    
    public string? AboutMe { get; set; }
    
    public TitleCodeResponse TitleCode { get; set; } = null!;
    
    public ICollection<CreateCertificationResponse> Certifications { get; set; } = new List<CreateCertificationResponse>();
    
    public ICollection<CreateEducationResponse> Educations { get; set; } = new List<CreateEducationResponse>();
    
    public ICollection<CreateHighlightResponse> Highlights { get; set; } = new List<CreateHighlightResponse>();
    
    public ICollection<CreateSkillResponse> Skills { get; set; } = new List<CreateSkillResponse>();
    
}