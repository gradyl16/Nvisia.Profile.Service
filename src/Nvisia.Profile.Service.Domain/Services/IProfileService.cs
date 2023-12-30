using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Domain.Services;

public interface IProfileService
{
    Task<ProfileDTO?> GetProfileByEmail(string email);
    Task<ProfileDTO?> GetProfileById(int id);
    Task<ProfileDTO?> CreateProfile(ProfileDTO profile);
    Task<ProfileDTO?> UpdateProfile(ProfileDTO profile);
    Task<ProfileDTO?> UpdateAboutMe(int profileId, string aboutMe);
}