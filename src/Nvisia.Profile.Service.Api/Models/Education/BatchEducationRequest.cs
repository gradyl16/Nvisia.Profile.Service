namespace Nvisia.Profile.Service.Api.Models.Education;

public class BatchEducationRequest
{
    public int ProfileId { get; set; }
    
    public ICollection<EducationRequest> Educations { get; set; } = new List<EducationRequest>();
}