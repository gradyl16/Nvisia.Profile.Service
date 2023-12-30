using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Controllers;
using Nvisia.Profile.Service.Api.Errors;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.UnitTests.Generators;

namespace Nvisia.Profile.Service.UnitTests.Api.Controllers;

/// <summary>
/// This is testing the Controller for the Profile Apis.
/// We are not testing the HttpClient or Json Mapping done by .NET (That's integration testing)
/// The ProfileService is mocked and injected into the controller in the constructor.
/// </summary>
public class ProfileControllerTest
{
    private static readonly Fixture Fixture = new();

    private readonly IProfileService _profileService = Substitute.For<IProfileService>();

    private readonly IValidator<CreateProfileRequest> _createProfileRequestValidator =
        Substitute.For<IValidator<CreateProfileRequest>>();

    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator =
        Substitute.For<IValidator<UpdateProfileRequest>>();

    private readonly IValidator<UpdateAboutMeRequest> _updateAboutMeRequestValidator =
        Substitute.For<IValidator<UpdateAboutMeRequest>>();

    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ProfileController _controller;

    public ProfileControllerTest()
    {
        _controller = new ProfileController(_profileService, _createProfileRequestValidator,
            _updateProfileRequestValidator, _updateAboutMeRequestValidator, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _profileService.ClearSubstitute();
        _createProfileRequestValidator.ClearSubstitute();
        _updateProfileRequestValidator.ClearSubstitute();
        _updateAboutMeRequestValidator.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task GetProfileById_IdIsNegative_ReturnsBadRequest()
    {
        // Arrange
        const int id = -1;

        // Act
        var result = await _controller.GetProfileById(id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var value = ((BadRequestObjectResult)result).Value as string;

        value.Should().Be(ControllerErrors.IdGreaterThanZero);

        await _profileService.DidNotReceive().GetProfileById(id);
    }

    [Test]
    public async Task GetProfileById_IdIsZero_ReturnsBadRequest()
    {
        // Arrange
        const int id = 0;

        // Act
        var result = await _controller.GetProfileById(id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var value = ((BadRequestObjectResult)result).Value as string;
        value.Should().Be(ControllerErrors.IdGreaterThanZero);

        await _profileService.DidNotReceive().GetProfileById(id);
    }

    [Test]
    public async Task GetProfileById_ProfileNull_ReturnsNotFound()
    {
        // Arrange
        const int id = 1;
        _profileService.GetProfileById(id).ReturnsNull();

        // Act
        var result = await _controller.GetProfileById(id);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var value = ((NotFoundObjectResult)result).Value as string;
        value.Should().Be(ControllerErrors.NotFound("Profile"));

        await _profileService.Received(1).GetProfileById(id);
        _mapper.DidNotReceive().Map<GetProfileResponse>(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task GetProfileById_ProfileService_ThrowsException()
    {
        // Arrange
        const int id = 1;
        _profileService.GetProfileById(id).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.GetProfileById(id);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileService.Received(1).GetProfileById(id);
        _mapper.DidNotReceive().Map<GetProfileResponse>(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task GetProfileById_Mapper_ThrowsException()
    {
        // Arrange
        const int id = 1;
        var profileDTO = Fixture.Create<ProfileDTO>();
        _profileService.GetProfileById(id).Returns(profileDTO);
        _mapper.Map<GetProfileResponse>(profileDTO).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.GetProfileById(id);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileService.Received(1).GetProfileById(id);
        _mapper.Received(1).Map<GetProfileResponse>(profileDTO);
    }

    [Test]
    public async Task GetProfileById_Successful()
    {
        // Arrange
        const int id = 1;
        var profileDTO = Fixture.Create<ProfileDTO>();
        var getProfileResponse = Fixture.Create<GetProfileResponse>();
        _profileService.GetProfileById(id).Returns(profileDTO);
        _mapper.Map<GetProfileResponse>(profileDTO).Returns(getProfileResponse);

        // Act
        var result = await _controller.GetProfileById(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();

        var value = ((OkObjectResult)result).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<GetProfileResponse>();

        var profile = value as GetProfileResponse;
        profile.Should().NotBeNull();
        profile!.ProfileId.Should().Be(getProfileResponse.ProfileId);
        profile.FirstName.Should().Be(getProfileResponse.FirstName);
        profile.LastName.Should().Be(getProfileResponse.LastName);
        profile.EmailAddress.Should().Be(getProfileResponse.EmailAddress);
        profile.YearsOfExperience.Should().Be(getProfileResponse.YearsOfExperience);
        profile.AboutMe.Should().Be(getProfileResponse.AboutMe);
        profile.TitleCode.Should().Be(getProfileResponse.TitleCode);

        await _profileService.Received(1).GetProfileById(id);
        _mapper.Received(1).Map<GetProfileResponse>(profileDTO);
    }

    [Test]
    public async Task GetProfileByEmail_EmailIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        const string email = "";

        // Act
        var result = await _controller.GetProfileByEmail(email);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var value = ((BadRequestObjectResult)result).Value as string;
        value.Should().Be(ControllerErrors.PropertyNotEmpty("Email"));

        await _profileService.DidNotReceive().GetProfileByEmail(email);
    }

    [Test]
    public async Task GetProfileByEmail_EmailIsWhitespace_ReturnsBadRequest()
    {
        // Arrange
        const string email = " ";

        // Act
        var result = await _controller.GetProfileByEmail(email);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var value = ((BadRequestObjectResult)result).Value as string;
        value.Should().Be(ControllerErrors.PropertyNotEmpty("Email"));

        await _profileService.DidNotReceive().GetProfileByEmail(email);
    }

    [Test]
    public async Task GetProfileByEmail_ProfileNull_ReturnsNotFound()
    {
        // Arrange
        const string email = "email@example.com";
        _profileService.GetProfileByEmail(email).ReturnsNull();

        // Act
        var result = await _controller.GetProfileByEmail(email);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var value = ((NotFoundObjectResult)result).Value as string;
        value.Should().Be(ControllerErrors.NotFound("Profile"));

        await _profileService.Received(1).GetProfileByEmail(email);
        _mapper.DidNotReceive().Map<GetProfileResponse>(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task GetProfileByEmail_ProfileService_ThrowsException()
    {
        // Arrange
        const string email = "email@example.com";
        _profileService.GetProfileByEmail(email).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.GetProfileByEmail(email);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileService.Received(1).GetProfileByEmail(email);
        _mapper.DidNotReceive().Map<GetProfileResponse>(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task GetProfileByEmail_Mapper_ThrowsException()
    {
        // Arrange
        const string email = "email@example.com";
        var profileDTO = Fixture.Create<ProfileDTO>();
        _profileService.GetProfileByEmail(email).Returns(profileDTO);
        _mapper.Map<GetProfileResponse>(profileDTO).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.GetProfileByEmail(email);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileService.Received(1).GetProfileByEmail(email);
        _mapper.Received(1).Map<GetProfileResponse>(profileDTO);
    }

    [Test]
    public async Task GetProfileByEmail_Successful()
    {
        // Arrange
        const string email = "email@example.com";
        var profileDTO = Fixture.Create<ProfileDTO>();
        var getProfileResponse = Fixture.Create<GetProfileResponse>();
        _profileService.GetProfileByEmail(email).Returns(profileDTO);
        _mapper.Map<GetProfileResponse>(profileDTO).Returns(getProfileResponse);

        // Act
        var result = await _controller.GetProfileByEmail(email);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();

        var value = ((OkObjectResult)result).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<GetProfileResponse>();

        var profile = value as GetProfileResponse;
        profile.Should().NotBeNull();
        profile!.ProfileId.Should().Be(getProfileResponse.ProfileId);
        profile.FirstName.Should().Be(getProfileResponse.FirstName);
        profile.LastName.Should().Be(getProfileResponse.LastName);
        profile.EmailAddress.Should().Be(getProfileResponse.EmailAddress);
        profile.YearsOfExperience.Should().Be(getProfileResponse.YearsOfExperience);
        profile.AboutMe.Should().Be(getProfileResponse.AboutMe);
        profile.TitleCode.Should().Be(getProfileResponse.TitleCode);

        await _profileService.Received(1).GetProfileByEmail(email);
        _mapper.Received(1).Map<GetProfileResponse>(profileDTO);
    }

    [Test]
    public async Task TestCreateProfile_ValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<CreateProfileRequest>();
        var validationResult = TestDataGenerator.GetFailedValidationResult();

        _createProfileRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var response = await _controller.CreateProfile(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        await _createProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.DidNotReceive().Map<ProfileDTO>(request);
    }

    [Test]
    public async Task TestCreateProfile_FirstMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<CreateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _createProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.CreateProfile(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _createProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.DidNotReceive().CreateProfile(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task TestCreateProfile_Service_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<CreateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();

        _createProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Returns(profileDTO);
        _profileService.CreateProfile(profileDTO).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.CreateProfile(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _createProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.Received(1).CreateProfile(profileDTO);
        _mapper.DidNotReceive().Map<CreateProfileResponse>(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task TestCreateProfile_Service_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<CreateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();

        _createProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Returns(profileDTO);
        _profileService.CreateProfile(profileDTO).ReturnsNull();

        // Act
        var response = await _controller.CreateProfile(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();

        await _createProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.Received(1).CreateProfile(profileDTO);
        _mapper.DidNotReceive().Map<CreateProfileResponse>(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task TestCreateProfile_SecondMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<CreateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();
        var createdProfile = Fixture.Create<ProfileDTO>();

        _createProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Returns(profileDTO);
        _profileService.CreateProfile(profileDTO).Returns(createdProfile);
        _mapper.Map<CreateProfileResponse>(createdProfile).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.CreateProfile(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _createProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.Received(1).CreateProfile(profileDTO);
        _mapper.Received(1).Map<CreateProfileResponse>(createdProfile);
    }

    [Test]
    public async Task TestCreateProfile_Successful()
    {
        // Arrange
        var request = Fixture.Create<CreateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();
        var createdProfile = Fixture.Create<ProfileDTO>();
        var createdProfileResponse = Fixture.Create<CreateProfileResponse>();

        _createProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Returns(profileDTO);
        _profileService.CreateProfile(profileDTO).Returns(createdProfile);
        _mapper.Map<CreateProfileResponse>(createdProfile).Returns(createdProfileResponse);

        // Act
        var response = await _controller.CreateProfile(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<CreatedResult>();

        var value = ((CreatedResult)response).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<CreateProfileResponse>();

        var profile = value as CreateProfileResponse;
        profile!.ProfileId.Should().Be(createdProfileResponse.ProfileId);
        profile.FirstName.Should().Be(createdProfileResponse.FirstName);
        profile.LastName.Should().Be(createdProfileResponse.LastName);
        profile.EmailAddress.Should().Be(createdProfileResponse.EmailAddress);
        profile.YearsOfExperience.Should().Be(createdProfileResponse.YearsOfExperience);
        profile.AboutMe.Should().Be(createdProfileResponse.AboutMe);
        profile.TitleCodeId.Should().Be(createdProfileResponse.TitleCodeId);

        await _createProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.Received(1).CreateProfile(profileDTO);
        _mapper.Received(1).Map<CreateProfileResponse>(createdProfile);
    }

    [Test]
    public async Task TestUpdateProfile_ValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<UpdateProfileRequest>();
        var validationResult = TestDataGenerator.GetFailedValidationResult();

        _updateProfileRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var response = await _controller.UpdateProfile(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        await _updateProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.DidNotReceive().Map<ProfileDTO>(request);
    }

    [Test]
    public async Task TestUpdateProfile_Mapper_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<UpdateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _updateProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.UpdateProfile(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _updateProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.DidNotReceive().UpdateProfile(Arg.Any<ProfileDTO>());
    }

    [Test]
    public async Task TestUpdateProfile_Service_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<UpdateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();
        
        _updateProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Returns(profileDTO);
        _profileService.UpdateProfile(profileDTO).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.UpdateProfile(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _updateProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.Received(1).UpdateProfile(profileDTO);
    }

    [Test]
    public async Task TestUpdateProfile_Service_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<UpdateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();
        
        _updateProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Returns(profileDTO);
        _profileService.UpdateProfile(profileDTO).ReturnsNull();

        // Act
        var response = await _controller.UpdateProfile(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();

        await _updateProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.Received(1).UpdateProfile(profileDTO);
    }

    [Test]
    public async Task TestUpdateProfile_Successful()
    {
        // Arrange
        var request = Fixture.Create<UpdateProfileRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();
        var updatedProfile = Fixture.Create<ProfileDTO>();
        
        _updateProfileRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ProfileDTO>(request).Returns(profileDTO);
        _profileService.UpdateProfile(profileDTO).Returns(updatedProfile);

        // Act
        var response = await _controller.UpdateProfile(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AcceptedResult>();

        await _updateProfileRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ProfileDTO>(request);
        await _profileService.Received(1).UpdateProfile(profileDTO);
    }
    
    [Test]
    public async Task TestUpdateAboutMe_ValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<UpdateAboutMeRequest>();
        var validationResult = TestDataGenerator.GetFailedValidationResult();

        _updateAboutMeRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var response = await _controller.UpdateAboutMe(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        await _updateAboutMeRequestValidator.Received(1).ValidateAsync(request);
        await _profileService.DidNotReceive().UpdateAboutMe(Arg.Any<int>(), Arg.Any<string>());
    }
    
    [Test]
    public async Task TestUpdateAboutMe_Service_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<UpdateAboutMeRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _updateAboutMeRequestValidator.ValidateAsync(request).Returns(validationResult);
        _profileService.UpdateAboutMe(request.ProfileId, request.AboutMe).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.UpdateAboutMe(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _updateAboutMeRequestValidator.Received(1).ValidateAsync(request);
        await _profileService.Received(1).UpdateAboutMe(request.ProfileId, request.AboutMe);
    }

    [Test]
    public async Task TestUpdateAboutMe_Service_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<UpdateAboutMeRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _updateAboutMeRequestValidator.ValidateAsync(request).Returns(validationResult);
        _profileService.UpdateAboutMe(request.ProfileId, request.AboutMe).ReturnsNull();

        // Act
        var response = await _controller.UpdateAboutMe(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();

        await _updateAboutMeRequestValidator.Received(1).ValidateAsync(request);
        await _profileService.Received(1).UpdateAboutMe(request.ProfileId, request.AboutMe);
    }

    [Test]
    public async Task TestUpdateAboutMe_Successful()
    {
        // Arrange
        var request = Fixture.Create<UpdateAboutMeRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var profileDTO = Fixture.Create<ProfileDTO>();
        
        _updateAboutMeRequestValidator.ValidateAsync(request).Returns(validationResult);
        _profileService.UpdateAboutMe(request.ProfileId, request.AboutMe).Returns(profileDTO);

        // Act
        var response = await _controller.UpdateAboutMe(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<NoContentResult>();

        await _updateAboutMeRequestValidator.Received(1).ValidateAsync(request);
        await _profileService.Received(1).UpdateAboutMe(request.ProfileId, request.AboutMe);
    }
}