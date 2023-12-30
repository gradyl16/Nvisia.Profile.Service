using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public interface IProfileRepository
{
    Task<ProfileEntity?> GetProfileById(int profileId);
    Task<ProfileEntity?> GetProfileByEmail(string email);
    Task<ProfileEntity?> SaveProfile(ProfileEntity profile);
    Task<ProfileEntity?> UpdateAboutMe(int profileId, string aboutMe);
}