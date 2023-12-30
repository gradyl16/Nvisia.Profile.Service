using AutoFixture;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Api.Validators.Skill;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.UnitTests.Api.Validators.Skill;

public class BatchSkillRequestValidatorTest
{
    private static readonly Fixture Fixture = new();
    private readonly ISkillCodeService _skillCodeService = Substitute.For<ISkillCodeService>();
    private readonly BatchSkillRequestValidator _validator;

    public BatchSkillRequestValidatorTest()
	{
		_validator = new BatchSkillRequestValidator(_skillCodeService);
	}

	[SetUp]
	public void Setup()
	{
		_skillCodeService.ClearSubstitute();
	}
    
    [TestCase(1, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(-1, ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public async Task<bool> TestValidate_ProfileId(int profileId)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ProfileId = profileId;

        _skillCodeService.GetSkillCodes().Returns(CreateSkillCodes());

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }
    
    [TestCase(2, ExpectedResult = false)]
    [TestCase(1, ExpectedResult = true)]
    [TestCase(0, ExpectedResult = false)]
    [TestCase(-1, ExpectedResult = false)]
    [TestCase(null, ExpectedResult = false)]
    public async Task<bool> TestValidate_SkillCodeId(int skillCodeId)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Skills = CreateSkills(skillCodeId: skillCodeId);

        _skillCodeService.GetSkillCodes().Returns(CreateSkillCodes());

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }

    [TestCase("Description", ExpectedResult = true)]
    [TestCase(null, ExpectedResult = false)]
    [TestCase("", ExpectedResult = false)]
    [TestCase("   ", ExpectedResult = false)]
    public async Task<bool> TestValidate_Description(string description)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Skills = CreateSkills(description: description);

        _skillCodeService.GetSkillCodes().Returns(CreateSkillCodes());

        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }


    [TestCase(0, ExpectedResult = false)]
    [TestCase(1, ExpectedResult = true)]
    [TestCase(6, ExpectedResult = true)]
    [TestCase(7, ExpectedResult = false)]
    [TestCase(10, ExpectedResult = false)]
    public async Task<bool> TestValidate_SkillsCount(int count)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Skills = CreateSkills(count: count);

        _skillCodeService.GetSkillCodes().Returns(CreateSkillCodes());
        
        var validationResult = await _validator.ValidateAsync(request);
        return validationResult.IsValid;
    }

    [Test]
    public async Task TestValidate_SkillsNull()
    {
        // Arrange
        var request = Fixture.Build<BatchSkillRequest>()
            .With(x => x.ProfileId, 1)
            .Without(x => x.Skills)
            .Create();

        _skillCodeService.GetSkillCodes().Returns(CreateSkillCodes());

        var validationResult = await _validator.ValidateAsync(request);
        validationResult.IsValid.Should().BeFalse();
    }
    
    [Test]
    public async Task TestValidate_Valid()
    {
        // Arrange
        var request = CreateValidRequest();

        _skillCodeService.GetSkillCodes().Returns(CreateSkillCodes());
        
        var validationResult = await _validator.ValidateAsync(request);
        validationResult.IsValid.Should().BeTrue();
    }

    private static BatchSkillRequest CreateValidRequest()
        => Fixture.Build<BatchSkillRequest>()
            .With(x => x.ProfileId, 1)
            .With(x => x.Skills, CreateSkills())
            .Create();

    private static List<SkillRequest> CreateSkills(int? skillCodeId = 1, string description = "Description", int count = 3)
        => Fixture.Build<SkillRequest>()
            .With(x => x.SkillCodeId, skillCodeId)
            .With(x => x.Description, description)
            .CreateMany(count).ToList();

    private static List<SkillCodeDTO> CreateSkillCodes(int? skillCodeId = 1)
        => Fixture.Build<SkillCodeDTO>()
            .With(x => x.SkillCodeId, skillCodeId)
            .CreateMany(3).ToList();

}