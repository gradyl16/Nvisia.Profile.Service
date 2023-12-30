namespace Nvisia.Profile.Service.Api.Models.Profile;

public class UpdateAboutMeRequest
{
    public int ProfileId { get; set; }
    
    public string AboutMe { get; set; } = null!;
}