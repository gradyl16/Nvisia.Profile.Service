namespace Nvisia.Profile.Service.Api.Models.Profile;

public class CreateProfileRequest
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public int TitleCodeId { get; set; }

    public int YearsOfExperience { get; set; }
}