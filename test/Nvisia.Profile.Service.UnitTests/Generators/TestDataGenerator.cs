using AutoFixture;
using FluentValidation.Results;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.UnitTests.Generators;

public static class TestDataGenerator
{
    private static readonly Fixture Fixture = new();

    private static T Create<T>(T source, Action<T>? modify = null)
    {
        modify?.Invoke(source);
        return source;
    }

    public static ProfileEntity CreateProfileEntity(Action<ProfileEntity>? modify = null) =>
        Create(
            Fixture
                .Build<ProfileEntity>()
                .WithAutoProperties()
                .Without(x => x.ProfileId)
                .With(x => x.EmailAddress, "email@example.com")
                .With(x => x.FirstName, "e")
                .With(x => x.LastName, "mail")
                .With(x => x.YearsOfExperience, 4)
                .With(x => x.AboutMe, "string")
                .With(x => x.Certifications, new List<CertificationEntity>())
                .With(x => x.Educations, new List<EducationEntity>())
                .With(x => x.Highlights, new List<HighlightEntity>())
                .With(x => x.Skills, new List<SkillEntity>())
                .Create(),
            modify);

    public static ValidationResult GetFailedValidationResult()
        => Fixture.Build<ValidationResult>().With(x => x.Errors, new List<ValidationFailure>
        {
            new()
            {
                ErrorMessage = "Test Error Message"
            }
        }).Create();
    
    public static ValidationResult GetPassedValidationResult()
        => Fixture.Build<ValidationResult>().With(x => x.Errors, new List<ValidationFailure>()).Create();
}