using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Models.Education;
using Nvisia.Profile.Service.Api.Validators.Education;

namespace Nvisia.Profile.Service.UnitTests.Api.Validators.Education;

public class BatchEducationRequestValidatorTest
{
    private static readonly Fixture Fixture = new();
    private readonly BatchEducationRequestValidator _validator = new();
    
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
    
     [TestCase("SchoolName", ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_SchoolName(string schoolName)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Educations = CreateEducations(schoolName: schoolName);

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
    
    [TestCase(2022, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(1890, ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public bool TestValidate_GraduationYear(int graduationYear)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Educations = CreateEducations(graduationYear: graduationYear);

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
    
    [TestCase("MajorDegreeName", ExpectedResult = true)]
    [TestCase(null, ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    public bool TestValidate_MajorDegreeName(string majorDegreeName)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Educations = CreateEducations(majorDegreeName: majorDegreeName);

        var validationResult = _validator.Validate(request);
        return validationResult.IsValid;
    }
    
    [TestCase("MinorDegreeName", ExpectedResult = true)]
    [TestCase(null, ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    public bool TestValidate_MinorDegreeName(string minorDegreeName)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Educations = CreateEducations(minorDegreeName: minorDegreeName);

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

    private static BatchEducationRequest CreateValidRequest()
        => Fixture.Build<BatchEducationRequest>()
            .With(x => x.ProfileId, 1)
            .With(x => x.Educations, CreateEducations())
            .Create();

    private static List<EducationRequest> CreateEducations(string? schoolName = "SchoolName", int graduationYear = 2022, string? majorDegreeName = "MajorDegreeName", string? minorDegreeName = "MinorDegreeName")
        => Fixture.Build<EducationRequest>()
            .With(x => x.SchoolName, schoolName)
            .With(x => x.MajorDegreeName, majorDegreeName)
            .With(x => x.MinorDegreeName, minorDegreeName)
            .With(x => x.GraduationYear, graduationYear)
            .CreateMany(3).ToList();
}