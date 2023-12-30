using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.UnitTests.Generators;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.Domain.Services;

[TestFixture]
public class ProfileServiceTest
{
    private static readonly Fixture Fixture = new();
    private readonly IProfileRepository _profileRepository = Substitute.For<IProfileRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    private readonly ProfileService _profileService;

    public ProfileServiceTest()
    {
        _profileService = new ProfileService(_profileRepository, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _profileRepository.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestGetProfileById_Repository_ThrowsException()
    {
        // Arrange
        const int id = 1;
        
        _profileRepository.GetProfileById(id).Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _profileService.GetProfileById(id);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileRepository.Received(1).GetProfileById(id);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }
    
    [Test]
    public async Task TestGetProfileById_Mapper_ThrowsException()
    {
        // Arrange
        const int id = 1;
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        
        _profileRepository.GetProfileById(id).Returns(profileEntity);
        _mapper.Map<ProfileDTO>(profileEntity).Throws(new Exception("Mapper Exception"));
        
        // Act
        var action = () => _profileService.GetProfileById(id);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileRepository.Received(1).GetProfileById(id);
        _mapper.Received(1).Map<ProfileDTO>(profileEntity);
    }
    
    [Test]
    public async Task TestGetProfileById_ReturnsNull_Successful()
    {
        // Arrange
        const int id = 1;
        
        _profileRepository.GetProfileById(id).ReturnsNull();

        // Act
        var result = await _profileService.GetProfileById(id);

        // Assert
        result.Should().BeNull();

        await _profileRepository.Received(1).GetProfileById(id);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }

    
    [Test]
    public async Task TestGetProfileById_ReturnsProfile_Successful()
    {
        // Arrange
        const int id = 1;
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        var profileDTO = Fixture.Create<ProfileDTO>();
        
        _profileRepository.GetProfileById(id).Returns(profileEntity);
        _mapper.Map<ProfileDTO>(profileEntity).Returns(profileDTO);
        
        // Act
        var result = await _profileService.GetProfileById(id);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().NotBeNull();
        result.FirstName.Should().Be(profileDTO.FirstName);
        result.LastName.Should().Be(profileDTO.LastName);
        result.EmailAddress.Should().Be(profileDTO.EmailAddress);
        result.YearsOfExperience.Should().Be(profileDTO.YearsOfExperience);
        result.AboutMe.Should().Be(profileDTO.AboutMe);
        result.TitleCodeId.Should().Be(profileDTO.TitleCodeId);

        await _profileRepository.Received(1).GetProfileById(id);
        _mapper.Received(1).Map<ProfileDTO>(profileEntity);
    }

    [Test]
    public async Task TestGetProfileByEmail_Repository_ThrowsException()
    {
        // Arrange
        const string email = "email@example.com";
        _profileRepository.GetProfileByEmail(email).Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _profileService.GetProfileByEmail(email);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileRepository.Received(1).GetProfileByEmail(email);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }
    
    [Test]
    public async Task TestGetProfileByEmail_Mapper_ThrowsException()
    {
        // Arrange
        const string email = "email@example.com";
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        
        _profileRepository.GetProfileByEmail(email).Returns(profileEntity);
        _mapper.Map<ProfileDTO>(profileEntity).Throws(new Exception("Mapper Exception"));
        
        // Act
        var action = () => _profileService.GetProfileByEmail(email);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileRepository.Received(1).GetProfileByEmail(email);
        _mapper.Received(1).Map<ProfileDTO>(profileEntity);
    }
    
    [Test]
    public async Task TestGetProfileByEmail_ReturnsNull_Successful()
    {
        // Arrange
        const string email = "email@example.com";
        _profileRepository.GetProfileByEmail(email).ReturnsNull();

        // Act
        var result = await _profileService.GetProfileByEmail(email);

        // Assert
        result.Should().BeNull();

        await _profileRepository.Received(1).GetProfileByEmail(email);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }

    
    [Test]
    public async Task TestGetProfileByEmail_ReturnsProfile_Successful()
    {
        // Arrange
        const string email = "email@example.com";
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        var profileDTO = Fixture.Create<ProfileDTO>();
        
        _profileRepository.GetProfileByEmail(email).Returns(profileEntity);
        _mapper.Map<ProfileDTO>(profileEntity).Returns(profileDTO);
        
        // Act
        var result = await _profileService.GetProfileByEmail(email);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().NotBeNull();
        result.FirstName.Should().Be(profileDTO.FirstName);
        result.LastName.Should().Be(profileDTO.LastName);
        result.EmailAddress.Should().Be(profileDTO.EmailAddress);
        result.YearsOfExperience.Should().Be(profileDTO.YearsOfExperience);
        result.AboutMe.Should().Be(profileDTO.AboutMe);
        result.TitleCodeId.Should().Be(profileDTO.TitleCodeId);

        await _profileRepository.Received(1).GetProfileByEmail(email);
        _mapper.Received(1).Map<ProfileDTO>(profileEntity);
    }

    [Test]
    public async Task TestCreateProfile_FirstMapper_ThrowsException()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();

        _mapper.Map<ProfileEntity>(profileDTO).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _profileService.CreateProfile(profileDTO);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.DidNotReceive().SaveProfile(Arg.Any<ProfileEntity>());
    }

    [Test]
    public async Task TestCreateProfile_Repository_ThrowsException()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _profileService.CreateProfile(profileDTO);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
    }

    [Test]
    public async Task TestCreateProfile_Repository_ReturnsNull()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).ReturnsNull();

        // Act
        var result = await _profileService.CreateProfile(profileDTO);

        // Assert
        result.Should().BeNull();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }

    [Test]
    public async Task TestCreateProfile_SecondMapper_ThrowsException()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        var createdProfileEntity = TestDataGenerator.CreateProfileEntity();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).Returns(createdProfileEntity);
        _mapper.Map<ProfileDTO>(createdProfileEntity).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _profileService.CreateProfile(profileDTO);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
        _mapper.Received(1).Map<ProfileDTO>(createdProfileEntity);
    }

    [Test]
    public async Task TestCreateProfile_Successful()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        var createdProfileEntity = TestDataGenerator.CreateProfileEntity();
        var createdProfileDTO = Fixture.Create<ProfileDTO>();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).Returns(createdProfileEntity);
        _mapper.Map<ProfileDTO>(createdProfileEntity).Returns(createdProfileDTO);

        // Act
        var result = await _profileService.CreateProfile(profileDTO);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().NotBeNull();
        result.FirstName.Should().Be(createdProfileDTO.FirstName);
        result.LastName.Should().Be(createdProfileDTO.LastName);
        result.EmailAddress.Should().Be(createdProfileDTO.EmailAddress);
        result.YearsOfExperience.Should().Be(createdProfileDTO.YearsOfExperience);
        result.AboutMe.Should().Be(createdProfileDTO.AboutMe);
        result.TitleCodeId.Should().Be(createdProfileDTO.TitleCodeId);

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
        _mapper.Received(1).Map<ProfileDTO>(createdProfileEntity);
    }

    [Test]
    public async Task TestUpdateProfile_FirstMapper_ThrowsException()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();

        _mapper.Map<ProfileEntity>(profileDTO).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _profileService.UpdateProfile(profileDTO);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.DidNotReceive().SaveProfile(Arg.Any<ProfileEntity>());
    }

    [Test]
    public async Task TestUpdateProfile_Repository_ThrowsException()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _profileService.UpdateProfile(profileDTO);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
    }

    [Test]
    public async Task TestUpdateProfile_Repository_ReturnsNull()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).ReturnsNull();

        // Act
        var result = await _profileService.UpdateProfile(profileDTO);

        // Assert
        result.Should().BeNull();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }

    [Test]
    public async Task TestUpdateProfile_SecondMapper_ThrowsException()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        var updatedProfileEntity = TestDataGenerator.CreateProfileEntity();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).Returns(updatedProfileEntity);
        _mapper.Map<ProfileDTO>(updatedProfileEntity).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _profileService.UpdateProfile(profileDTO);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
        _mapper.Received(1).Map<ProfileDTO>(updatedProfileEntity);
    }

    [Test]
    public async Task TestUpdateProfile_Successful()
    {
        // Arrange
        var profileDTO = Fixture.Create<ProfileDTO>();
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        var updatedProfileEntity = TestDataGenerator.CreateProfileEntity();
        var updatedProfileDTO = Fixture.Create<ProfileDTO>();

        _mapper.Map<ProfileEntity>(profileDTO).Returns(profileEntity);
        _profileRepository.SaveProfile(profileEntity).Returns(updatedProfileEntity);
        _mapper.Map<ProfileDTO>(updatedProfileEntity).Returns(updatedProfileDTO);

        // Act
        var result = await _profileService.UpdateProfile(profileDTO);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().NotBeNull();
        result.FirstName.Should().Be(updatedProfileDTO.FirstName);
        result.LastName.Should().Be(updatedProfileDTO.LastName);
        result.EmailAddress.Should().Be(updatedProfileDTO.EmailAddress);
        result.YearsOfExperience.Should().Be(updatedProfileDTO.YearsOfExperience);
        result.AboutMe.Should().Be(updatedProfileDTO.AboutMe);
        result.TitleCodeId.Should().Be(updatedProfileDTO.TitleCodeId);

        _mapper.Received(1).Map<ProfileEntity>(profileDTO);
        await _profileRepository.Received(1).SaveProfile(profileEntity);
        _mapper.Received(1).Map<ProfileDTO>(updatedProfileEntity);
    }
    
    [Test]
    public async Task TestUpdateAboutMe_Repository_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        const string aboutMe = "AboutMe";

        _profileRepository.UpdateAboutMe(profileId, aboutMe).Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _profileService.UpdateAboutMe(profileId, aboutMe);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileRepository.Received(1).UpdateAboutMe(profileId, aboutMe);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }

    [Test]
    public async Task TestUpdateAboutMe_Repository_ReturnsNull()
    {
        // Arrange
        const int profileId = 1;
        const string aboutMe = "AboutMe";

        _profileRepository.UpdateAboutMe(profileId, aboutMe).ReturnsNull();

        // Act
        var result = await _profileService.UpdateAboutMe(profileId, aboutMe);

        // Assert
        result.Should().BeNull();

        await _profileRepository.Received(1).UpdateAboutMe(profileId, aboutMe);
        _mapper.DidNotReceive().Map<ProfileDTO>(Arg.Any<ProfileEntity>());
    }

    [Test]
    public async Task TestUpdateAboutMe_Mapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        const string aboutMe = "AboutMe";
        var profileEntity = TestDataGenerator.CreateProfileEntity();

        _profileRepository.UpdateAboutMe(profileId, aboutMe).Returns(profileEntity);
        _mapper.Map<ProfileDTO>(profileEntity).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _profileService.UpdateAboutMe(profileId, aboutMe);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _profileRepository.Received(1).UpdateAboutMe(profileId, aboutMe);
        _mapper.Received(1).Map<ProfileDTO>(profileEntity);
    }

    [Test]
    public async Task TestUpdateAboutMe_Successful()
    {
        // Arrange
        const int profileId = 1;
        const string aboutMe = "AboutMe";
        var profileEntity = TestDataGenerator.CreateProfileEntity();
        var profileDTO = Fixture.Create<ProfileDTO>();
        
        _profileRepository.UpdateAboutMe(profileId, aboutMe).Returns(profileEntity);
        _mapper.Map<ProfileDTO>(profileEntity).Returns(profileDTO);

        // Act
        var result = await _profileService.UpdateAboutMe(profileId, aboutMe);

        // Assert
        result.Should().NotBeNull();
        result!.ProfileId.Should().NotBeNull();
        result.FirstName.Should().Be(profileDTO.FirstName);
        result.LastName.Should().Be(profileDTO.LastName);
        result.EmailAddress.Should().Be(profileDTO.EmailAddress);
        result.YearsOfExperience.Should().Be(profileDTO.YearsOfExperience);
        result.AboutMe.Should().Be(profileDTO.AboutMe);
        result.TitleCodeId.Should().Be(profileDTO.TitleCodeId);

        await _profileRepository.Received(1).UpdateAboutMe(profileId, aboutMe);
        _mapper.Received(1).Map<ProfileDTO>(profileEntity);
    }
}