using AutoFixture;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Api.Validators.Profile;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;


namespace Nvisia.Profile.Service.UnitTests.Api.Validators.Profile;

public class CreateProfileRequestValidatorTest
{
    private static readonly Fixture Fixture = new();
    private readonly ITitleCodeService _titleCodeService = Substitute.For<ITitleCodeService>();
    private readonly CreateProfileRequestValidator _validator;

    public CreateProfileRequestValidatorTest()
	{
		_validator = new CreateProfileRequestValidator(_titleCodeService);
	}

	[SetUp]
	public void Setup()
	{
		_titleCodeService.ClearSubstitute();
	}

    [TestCase("FirstName", ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public async Task<bool> TestValidate_FirstName(string firstName)
    {
        // Arrange
        var request = CreateValidRequest();
        request.FirstName = firstName;

        _titleCodeService.GetTitleCodes().Returns(CreateTitleCodes([1]));

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }
    
    [TestCase("LastName", ExpectedResult = true)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public async Task<bool> TestValidate_LastName(string lastName)
    {
        // Arrange
        var request = CreateValidRequest();
        request.LastName = lastName;

        _titleCodeService.GetTitleCodes().Returns(CreateTitleCodes([1]));

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }
    
    [TestCase("emailAddress@gmail.com", ExpectedResult = true)]
    [TestCase("emailAddress", ExpectedResult = false)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public async Task<bool> TestValidate_EmailAddress(string emailAddress)
    {
        // Arrange
        var request = CreateValidRequest();
        request.EmailAddress = emailAddress;

        _titleCodeService.GetTitleCodes().Returns(CreateTitleCodes([1]));

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }
    
    [TestCase(4, ExpectedResult = false)]
    [TestCase(3, ExpectedResult = true)]
    [TestCase(2, ExpectedResult = false)]
    [TestCase(1, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(-1, ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public async Task<bool> TestValidate_TitleCodeId(int titleCodeId)
    {
        // Arrange
        var request = CreateValidRequest();
        request.TitleCodeId = titleCodeId;

        _titleCodeService.GetTitleCodes().Returns(CreateTitleCodes([1, 3]));

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }
    
    [TestCase(2, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = true)]
    [TestCase(-1, ExpectedResult = false)]
    public async Task<bool> TestValidate_YearsOfExperience(int yearsOfExperience)
    {
        // Arrange
        var request = CreateValidRequest();
        request.YearsOfExperience = yearsOfExperience;

        _titleCodeService.GetTitleCodes().Returns(CreateTitleCodes([1]));

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }
    
    [Test]
    public async Task TestValidate_Valid()
    {
        // Arrange
        var request = CreateValidRequest();

        _titleCodeService.GetTitleCodes().Returns(CreateTitleCodes([1]));

        var validationResult = await _validator.ValidateAsync(request);
        validationResult.IsValid.Should().BeTrue();
    }

    private static CreateProfileRequest CreateValidRequest()
        => Fixture.Build<CreateProfileRequest>()
            .With(x => x.FirstName, "firstName")
            .With(x => x.LastName, "lastName")
            .With(x => x.EmailAddress, "emailAddress@gmail.com")
            .With(x => x.TitleCodeId, 1)
            .With(x => x.YearsOfExperience, 4)
            .Create();

    private static List<TitleCodeDTO> CreateTitleCodes(int[] titleCodeIds)
        => titleCodeIds.SelectMany(id => 
            Fixture.Build<TitleCodeDTO>()
                .With(x => x.TitleCodeId, id)
                .CreateMany(1).ToList()
        ).ToList();
}