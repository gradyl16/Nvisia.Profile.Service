using AutoMapper;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IMapper _mapper;

    public ProfileService(IProfileRepository profileRepository, IMapper mapper)
    {
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<ProfileDTO?> GetProfileById(int id)
    {
        var profileEntity = await _profileRepository.GetProfileById(id);
        return profileEntity is null 
            ? null 
            : _mapper.Map<ProfileDTO>(profileEntity);
    }

    public async Task<ProfileDTO?> GetProfileByEmail(string email)
    {
        var profileEntity = await _profileRepository.GetProfileByEmail(email);
        return profileEntity is null 
            ? null 
            : _mapper.Map<ProfileDTO>(profileEntity);
    }
    
    public async Task<ProfileDTO?> CreateProfile(ProfileDTO profile)
    {
        var profileEntity = _mapper.Map<ProfileEntity>(profile);
        var createdProfile = await _profileRepository.SaveProfile(profileEntity);
        return createdProfile is null 
            ? null 
            : _mapper.Map<ProfileDTO>(createdProfile);
    }

    public async Task<ProfileDTO?> UpdateProfile(ProfileDTO profile)
    {
        var profileEntity = _mapper.Map<ProfileEntity>(profile);
        var updatedProfile = await _profileRepository.SaveProfile(profileEntity);
        return updatedProfile is null 
            ? null 
            : _mapper.Map<ProfileDTO>(updatedProfile);
    }
    
    public async Task<ProfileDTO?> UpdateAboutMe(int profileId, string aboutMe)
    {
        var updatedProfile = await _profileRepository.UpdateAboutMe(profileId, aboutMe);
        return updatedProfile is null 
            ? null 
            : _mapper.Map<ProfileDTO>(updatedProfile);
    }
}