using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Context;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProfileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Get the entire profile (all children), READ-ONLY, by the primary key
    /// </summary>
    /// <param name="profileId"></param>
    /// <returns>Profile or Null</returns>
    public async Task<ProfileEntity?> GetProfileById(int profileId)
        => await _dbContext.Profiles
            .AsNoTrackingWithIdentityResolution()
            .Include(i => i.Certifications)
            .Include(i => i.Educations)
            .Include(i => i.Highlights)
            .Include(i => i.Skills.OrderBy(c => c.SortOrder)).ThenInclude(c => c.SkillCode)
            .Include(i => i.TitleCode)
            .FirstOrDefaultAsync(x => x.ProfileId == profileId);

    /// <summary>
    /// Get the entire profile (all children), READ-ONLY, by the email address
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Profile or Null</returns>
    public async Task<ProfileEntity?> GetProfileByEmail(string email)
    => await _dbContext.Profiles
            .AsNoTrackingWithIdentityResolution()
            .Include(i => i.Certifications)
            .Include(i => i.Educations)
            .Include(i => i.Highlights)
            .Include(i => i.Skills.OrderBy(c => c.SortOrder)).ThenInclude(c => c.SkillCode)
            .Include(i => i.TitleCode)
            .FirstOrDefaultAsync(x => String.Equals(x.EmailAddress, email, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Insert the profile, either by Inserting if there is no key (ProfileId is 0) or updating.
    /// </summary>
    /// <param name="profile"></param>
    /// <returns>Profile</returns>
    public async Task<ProfileEntity?> SaveProfile(ProfileEntity profile)
    {
        if (profile.ProfileId == 0) {
            // Create
            await _dbContext.Profiles.AddAsync(profile);
        } else {
            // Update, throw if ProfileId DNE
            var existingProfile = await GetProfileByIdForUpdate(profile.ProfileId);

            if (existingProfile is null)
                return null;

            profile.AboutMe = existingProfile.AboutMe;
            _dbContext.Profiles.Update(profile);
        }
        
        await _dbContext.SaveChangesAsync();

        return profile;
    }

    /// <summary>
    /// Get the profile, update about me and save
    /// </summary>
    /// <param name="profileId"></param>
    /// <param name="aboutMe"></param>
    /// <returns>Profile</returns>
    public async Task<ProfileEntity?> UpdateAboutMe(int profileId, string aboutMe)
    {
        var profile = await GetProfileByIdForUpdate(profileId);
        if (profile is null)
            return null;
        
        profile.AboutMe = aboutMe;
        
        _dbContext.Profiles.Update(profile);
        await _dbContext.SaveChangesAsync();
        
        return profile;
    }

    private async Task<ProfileEntity?> GetProfileByIdForUpdate(int profileId)
        => await _dbContext.Profiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProfileId == profileId);
}