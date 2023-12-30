using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Models.Certification;
using Nvisia.Profile.Service.Api.Validators.Certification;

namespace Nvisia.Profile.Service.UnitTests.Api.Validators.Certification;

public class BatchCertificationRequestValidatorTest
{
    private static readonly Fixture Fixture = new();
    private readonly BatchCertificationRequestValidator _validator = new();
    
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
    
    
    [TestCase("Title", ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_Title(string title)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Certifications = CreateCertifications(title: title);

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
    
    [TestCase(2022, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(1890, ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_Year(int year)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Certifications = CreateCertifications(year: year);

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
    
    [Test]
    public void TestValidate_Certifications_Empty()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Certifications = new List<CertificationRequest>();

        var validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeTrue();
    }
    
    [Test]
    public void TestValidate_Valid()
    {
        // Arrange
        var request = CreateValidRequest();
        
        var validationResult = _validator.Validate(request);
        validationResult.IsValid.Should().BeTrue();
    }

    private static BatchCertificationRequest CreateValidRequest()
        => Fixture.Build<BatchCertificationRequest>()
            .With(x => x.ProfileId, 1)
            .With(x => x.Certifications, CreateCertifications())
            .Create();

    private static List<CertificationRequest> CreateCertifications(string? title = "title", int year = 2022)
        => Fixture.Build<CertificationRequest>()
            .With(x => x.Title, title)
            .With(x => x.Year, year)
            .CreateMany(3).ToList();
}