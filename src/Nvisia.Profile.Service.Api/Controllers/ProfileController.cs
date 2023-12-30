using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nvisia.Profile.Service.Api.Constants;
using Nvisia.Profile.Service.Api.Errors;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Api.Controllers;

[ApiController]
[Route(ControllerConstants.Profile.BaseRoute)]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IValidator<CreateProfileRequest> _createProfileRequestValidator;
    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator;
    private readonly IValidator<UpdateAboutMeRequest> _updateAboutMeRequestValidator;
    private readonly IMapper _mapper;

    public ProfileController(IProfileService profileService,
        IValidator<CreateProfileRequest> createProfileRequestValidator,
        IValidator<UpdateProfileRequest> updateProfileRequestValidator,
        IValidator<UpdateAboutMeRequest> updateAboutMeRequestValidator,
        IMapper mapper)
    {
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        _createProfileRequestValidator = createProfileRequestValidator ??
                                         throw new ArgumentNullException(nameof(createProfileRequestValidator));
        _updateProfileRequestValidator = updateProfileRequestValidator ??
                                         throw new ArgumentNullException(nameof(updateProfileRequestValidator));
        _updateAboutMeRequestValidator = updateAboutMeRequestValidator ??
                                         throw new ArgumentNullException(nameof(updateAboutMeRequestValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet(ControllerConstants.IdRoute)]
    [ProducesResponseType(typeof(GetProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileById([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest(ControllerErrors.IdGreaterThanZero);

        var profileDTO = await _profileService.GetProfileById(id);
        if (profileDTO is null)
            return NotFound(ControllerErrors.NotFound("Profile"));

        var profileResponse = _mapper.Map<GetProfileResponse>(profileDTO);
        return Ok(profileResponse);
    }

    [HttpGet]
    [ProducesResponseType(typeof(GetProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfileByEmail([FromQuery] string email)
    {
        if (String.IsNullOrWhiteSpace(email)) {
            return BadRequest(ControllerErrors.PropertyNotEmpty("Email"));
        }

        var profileDTO = await _profileService.GetProfileByEmail(email);
        if (profileDTO is null)
            return NotFound(ControllerErrors.NotFound("Profile"));

        var profileResponse = _mapper.Map<GetProfileResponse>(profileDTO);
        return Ok(profileResponse);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest profile)
    {
        var validationResult = await _createProfileRequestValidator.ValidateAsync(profile);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var profileDTO = _mapper.Map<ProfileDTO>(profile);
        var createdProfile = await _profileService.CreateProfile(profileDTO);
        if (createdProfile is null)
            return BadRequest();

        var createdProfileResponse = _mapper.Map<CreateProfileResponse>(createdProfile);

        return Created($"/{createdProfileResponse.ProfileId}", createdProfileResponse);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest profile)
    {
        var validationResult = await _updateProfileRequestValidator.ValidateAsync(profile);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var profileDTO = _mapper.Map<ProfileDTO>(profile);
        var updatedProfile = await _profileService.UpdateProfile(profileDTO);
        if (updatedProfile is null)
            return BadRequest();

        return Accepted();
    }

    [HttpPut(ControllerConstants.Profile.AboutRoute)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAboutMe([FromBody] UpdateAboutMeRequest request)
    {
        var validationResult = await _updateAboutMeRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var updatedProfile = await _profileService.UpdateAboutMe(request.ProfileId, request.AboutMe);
        if (updatedProfile is null)
            return BadRequest();

        return NoContent();
    }
}