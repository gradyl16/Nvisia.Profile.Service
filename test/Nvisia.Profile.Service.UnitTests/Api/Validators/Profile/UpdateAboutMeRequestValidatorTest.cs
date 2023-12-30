using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Api.Validators.Profile;

namespace Nvisia.Profile.Service.UnitTests.Api.Validators.Profile;

public class UpdateAboutMeRequestValidatorTest
{
    private static readonly Fixture Fixture = new();
    private readonly UpdateAboutMeRequestValidator _validator = new();
    
    [TestCase(1, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(-1, ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_ProfileId(int profileId)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ProfileId = profileId;

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }

    
    [TestCase("aboutMe", ExpectedResult = true)]
    [TestCase(null, ExpectedResult = false)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    public bool TestValidate_AboutMe(string aboutMe)
    {
        // Arrange
        var request = CreateValidRequest();
        request.AboutMe = aboutMe;

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
   
    
    [Test]
    public void TestValidate_Valid()
    {
        // Arrange
        var request = CreateValidRequest();

        var validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeTrue();
    }

    private static UpdateAboutMeRequest CreateValidRequest()
        => Fixture.Build<UpdateAboutMeRequest>()
            .With(x => x.ProfileId, 1)
            .With(x => x.AboutMe, "aboutMe")
            .Create();
}