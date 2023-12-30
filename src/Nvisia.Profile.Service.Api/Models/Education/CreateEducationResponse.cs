namespace Nvisia.Profile.Service.Api.Models.Education;

public class CreateEducationResponse
{
    public int EducationId { get; set; }
    
    public string SchoolName { get; set; } = null!;
    
    public int GraduationYear { get; set; }
    
    public string? MajorDegreeName { get; set; }
    
    public string? MinorDegreeName { get; set; }
}