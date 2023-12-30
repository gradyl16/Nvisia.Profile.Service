using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Nvisia.Profile.Service.UnitTests.Generators;
using Nvisia.Profile.Service.UnitTests.Utils;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.WriteStore.Repositories;

/// <summary>
/// Test the Profile Repository using the in-memory database. <b>The in-memory cannot be used to test sql queries, only linq queries</b>.
/// These unit tests are to validate our entities and the linq queries. SQL Query testing should be done in integration with the target DB.
/// We create a new context between inserting the data to make sure it's persisted, and there are no side-effects.
/// </summary>
public class ProfileRepositoryTest
{
    [Test]
    public async Task TestGetProfileById()
    {
        const string dbName = "Get_Profile_By_Id";
        ProfileEntity? profileEntity;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            profileEntity = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profileEntity);
            await context.SaveChangesAsync();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profileRepository = new ProfileRepository(context);
            var profile = await profileRepository.GetProfileById(1);

            context.Entry(profile!).State.Should().Be(EntityState.Detached);
            profile.Should().NotBeNull();
            profile!.ProfileId.Should().Be(profileEntity.ProfileId);
            profile.FirstName.Should().Be(profileEntity.FirstName);
            profile.LastName.Should().Be(profileEntity.LastName);
            profile.EmailAddress.Should().Be(profileEntity.EmailAddress);
            profile.YearsOfExperience.Should().Be(profileEntity.YearsOfExperience);
            profile.AboutMe.Should().Be(profileEntity.AboutMe);
            profile.TitleCodeId.Should().Be(profileEntity.TitleCodeId);
        }
    }

    [Test]
    public async Task TestGetProfileByEmail()
    {
        const string dbName = "Get_Profile_By_Email";
        ProfileEntity? profileEntity;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            profileEntity = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profileEntity);
            await context.SaveChangesAsync();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profileRepository = new ProfileRepository(context);
            var profile = await profileRepository.GetProfileByEmail("email@example.com");

            context.Entry(profile!).State.Should().Be(EntityState.Detached);
            profile.Should().NotBeNull();
            profile!.ProfileId.Should().Be(profileEntity.ProfileId);
            profile.FirstName.Should().Be(profileEntity.FirstName);
            profile.LastName.Should().Be(profileEntity.LastName);
            profile.EmailAddress.Should().Be(profileEntity.EmailAddress);
            profile.YearsOfExperience.Should().Be(profileEntity.YearsOfExperience);
            profile.AboutMe.Should().Be(profileEntity.AboutMe);
            profile.TitleCodeId.Should().Be(profileEntity.TitleCodeId);
        }
    }

    [Test]
    public async Task TestGetProfileByEmailUppercase()
    {
        const string dbName = "Get_Profile_By_Email_Uppercase";
        ProfileEntity? profileEntity;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            profileEntity = TestDataGenerator.CreateProfileEntity();
            await context.AddAsync(profileEntity);
            await context.SaveChangesAsync();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profileRepository = new ProfileRepository(context);
            var profile = await profileRepository.GetProfileByEmail("EMAIL@example.COM");

            context.Entry(profile!).State.Should().Be(EntityState.Detached);
            profile.Should().NotBeNull();
            profile!.ProfileId.Should().Be(profileEntity.ProfileId);
            profile.FirstName.Should().Be(profileEntity.FirstName);
            profile.LastName.Should().Be(profileEntity.LastName);
            profile.EmailAddress.Should().Be(profileEntity.EmailAddress);
            profile.YearsOfExperience.Should().Be(profileEntity.YearsOfExperience);
            profile.AboutMe.Should().Be(profileEntity.AboutMe);
            profile.TitleCodeId.Should().Be(profileEntity.TitleCodeId);
        }
    }
    
    [Test]
    public async Task TestInsertProfile()
    {
        const string dbName = "Insert_Profile";
        ProfileEntity? profileEntity;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            var profileRepository = new ProfileRepository(context);
            profileEntity = await profileRepository.SaveProfile(profile);
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile =
                await context.Profiles.FirstOrDefaultAsync(
                    x => x.ProfileId == profileEntity!.ProfileId);

            profile.Should().NotBeNull();
            profile!.ProfileId.Should().Be(profileEntity!.ProfileId);
            profile.FirstName.Should().Be(profileEntity.FirstName);
            profile.LastName.Should().Be(profileEntity.LastName);
            profile.EmailAddress.Should().Be(profileEntity.EmailAddress);
            profile.YearsOfExperience.Should().Be(profileEntity.YearsOfExperience);
            profile.AboutMe.Should().Be(profileEntity.AboutMe);
            profile.TitleCodeId.Should().Be(profileEntity.TitleCodeId);
        }
    }

    [Test]
    public async Task TestUpdateProfile()
    {
        const string dbName = "Update_Profile";
        ProfileEntity? profileEntity;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            profileEntity = TestDataGenerator.CreateProfileEntity();
            var profileRepository = new ProfileRepository(context);
            await context.Profiles.AddAsync(profileEntity);
            await context.SaveChangesAsync();
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profileRepository = new ProfileRepository(context);

            profileEntity.FirstName = "Bob";
            profileEntity.LastName = "Doe";
            profileEntity.EmailAddress = "bdoe@example.com";
            profileEntity.YearsOfExperience = 50;
            profileEntity.TitleCodeId = 2;

            profileEntity = await profileRepository.SaveProfile(profileEntity);
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile =
                await context.Profiles.FirstOrDefaultAsync(
                    x => x.ProfileId == profileEntity!.ProfileId);

            profile.Should().NotBeNull();
            profile!.ProfileId.Should().Be(profileEntity!.ProfileId);
            profile.FirstName.Should().Be(profileEntity.FirstName);
            profile.LastName.Should().Be(profileEntity.LastName);
            profile.EmailAddress.Should().Be(profileEntity.EmailAddress);
            profile.YearsOfExperience.Should().Be(profileEntity.YearsOfExperience);
            profile.AboutMe.Should().Be(profileEntity.AboutMe);
            profile.TitleCodeId.Should().Be(profileEntity.TitleCodeId);
        }
    }

    [Test]
    public async Task TestUpdateProfile_ReturnsNull()
    {
        const string dbName = "Update_Profile_ReturnsNull";
        const int testProfileId = 1;
        ProfileEntity? profileEntity;
        await using (var context = TestUtils.CreateContext(dbName))
        {
            profileEntity = TestDataGenerator.CreateProfileEntity();
            var profileRepository = new ProfileRepository(context);
        }

        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profileRepository = new ProfileRepository(context);

            profileEntity.ProfileId = testProfileId;
            profileEntity.FirstName = "Bob";
            profileEntity.LastName = "Doe";
            profileEntity.EmailAddress = "bdoe@example.com";
            profileEntity.YearsOfExperience = 50;
            profileEntity.TitleCodeId = 2;

            profileEntity = await profileRepository.SaveProfile(profileEntity);

            profileEntity.Should().BeNull();
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile =
                await context.Profiles.FirstOrDefaultAsync(
                    x => x.ProfileId == testProfileId);

            profile.Should().BeNull();
        }
    }
    
    [Test]
    public async Task TestUpdateAboutMe()
    {
        const string dbName = "Update_AboutMe";
        ProfileEntity? profileEntity;
        var aboutMe = "This is my about me";
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profile = TestDataGenerator.CreateProfileEntity();
            var profileRepository = new ProfileRepository(context);
            profileEntity = await profileRepository.SaveProfile(profile);
        }
        
        await using (var context = TestUtils.CreateContext(dbName))
        {
            var profileRepository = new ProfileRepository(context);
            var profile = await profileRepository.UpdateAboutMe(profileEntity!.ProfileId, aboutMe);
            profile.Should().NotBeNull();
            profile!.ProfileId.Should().Be(profileEntity.ProfileId);
            profile.FirstName.Should().Be(profileEntity.FirstName);
            profile.LastName.Should().Be(profileEntity.LastName);
            profile.EmailAddress.Should().Be(profileEntity.EmailAddress);
            profile.YearsOfExperience.Should().Be(profileEntity.YearsOfExperience);
            profile.AboutMe.Should().NotBe(profileEntity.AboutMe);
            profile.AboutMe.Should().Be(aboutMe);
            profile.TitleCodeId.Should().Be(profileEntity.TitleCodeId);
        }
    }
}