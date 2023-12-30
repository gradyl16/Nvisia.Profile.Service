using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Controllers;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.UnitTests.Generators;

namespace Nvisia.Profile.Service.UnitTests.Api.Controllers;

public class SkillControllerTest
{
	private static readonly Fixture Fixture = new();
	private readonly ISkillService _skillService = Substitute.For<ISkillService>();
	private readonly IValidator<BatchSkillRequest> _batchCreateSkillRequestValidator =
        Substitute.For<IValidator<BatchSkillRequest>>();
	private readonly IMapper _mapper = Substitute.For<IMapper>();
	private readonly SkillController _controller;

	public SkillControllerTest()
	{
		_controller = new SkillController(_skillService, _batchCreateSkillRequestValidator, _mapper);
	}

	[SetUp]
	public void Setup()
	{
		_skillService.ClearSubstitute();
		_batchCreateSkillRequestValidator.ClearSubstitute();
		_mapper.ClearSubstitute();
	}

	[Test]
    public async Task TestBatchSkills_ValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchSkillRequest>();
        var validationResult = TestDataGenerator.GetFailedValidationResult();

        _batchCreateSkillRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var response = await _controller.BatchSkills(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        await _batchCreateSkillRequestValidator.Received(1).ValidateAsync(request);
        _mapper.DidNotReceive().Map<ICollection<SkillDTO>>(request);
    }

    [Test]
    public async Task TestBatchSkills_FirstMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchSkillRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _batchCreateSkillRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<SkillDTO>>(request.Skills).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchSkills(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateSkillRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<SkillDTO>>(request.Skills);
        await _skillService.DidNotReceive().BatchSkills(Arg.Any<int>(), Arg.Any<ICollection<SkillDTO>>());
    }

    [Test]
    public async Task TestBatchSkills_Service_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchSkillRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();

        _batchCreateSkillRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<SkillDTO>>(request.Skills).Returns(skillDTOs);
        _skillService.BatchSkills(request.ProfileId, skillDTOs).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.BatchSkills(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateSkillRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<SkillDTO>>(request.Skills);
        await _skillService.Received(1).BatchSkills(request.ProfileId, skillDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateSkillResponse>>(Arg.Any<ICollection<SkillDTO>>());
    }

    [Test]
    public async Task TestBatchSkills_Service_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchSkillRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();

        _batchCreateSkillRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<SkillDTO>>(request.Skills).Returns(skillDTOs);
        _skillService.BatchSkills(request.ProfileId, skillDTOs)
            .Returns(new List<SkillDTO>());

        // Act
        var response = await _controller.BatchSkills(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();

        await _batchCreateSkillRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<SkillDTO>>(request.Skills);
        await _skillService.Received(1).BatchSkills(request.ProfileId, skillDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateSkillResponse>>(Arg.Any<ICollection<SkillDTO>>());
    }

    [Test]
    public async Task TestBatchSkills_SecondMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchSkillRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();
        var createdSkills = Fixture.Create<ICollection<SkillDTO>>();

        _batchCreateSkillRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<SkillDTO>>(request.Skills).Returns(skillDTOs);
        _skillService.BatchSkills(request.ProfileId, skillDTOs).Returns(createdSkills);
        _mapper.Map<ICollection<CreateSkillResponse>>(Arg.Any<ICollection<SkillDTO>>())
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchSkills(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateSkillRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<SkillDTO>>(request.Skills);
        await _skillService.Received(1).BatchSkills(request.ProfileId, skillDTOs);
        _mapper.Received(1).Map<ICollection<CreateSkillResponse>>(Arg.Any<ICollection<SkillDTO>>());
    }

    [Test]
    public async Task TestBatchSkills_Successful()
    {
        // Arrange
        var request = Fixture.Create<BatchSkillRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();
        var createdSkills = Fixture.Create<ICollection<SkillDTO>>();
        var createdSkillResponses = Fixture.CreateMany<CreateSkillResponse>(3).ToList();

        _batchCreateSkillRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<SkillDTO>>(request.Skills).Returns(skillDTOs);
        _skillService.BatchSkills(request.ProfileId, skillDTOs).Returns(createdSkills);
        _mapper.Map<ICollection<CreateSkillResponse>>(Arg.Any<ICollection<SkillDTO>>())
            .Returns(createdSkillResponses);

        // Act
        var response = await _controller.BatchSkills(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AcceptedResult>();

        var value = ((AcceptedResult)response).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<List<CreateSkillResponse>>();

        var skills = value as List<CreateSkillResponse>;
        skills![0].SkillId.Should().Be(createdSkillResponses[0].SkillId);
        skills[0].SkillCodeCode.Should().Be(createdSkillResponses[0].SkillCodeCode);
        skills[0].SkillCodeLabel.Should().Be(createdSkillResponses[0].SkillCodeLabel);

        skills[1].SkillId.Should().Be(createdSkillResponses[1].SkillId);
        skills[1].SkillCodeCode.Should().Be(createdSkillResponses[1].SkillCodeCode);
        skills[1].SkillCodeLabel.Should().Be(createdSkillResponses[1].SkillCodeLabel);

        skills[2].SkillId.Should().Be(createdSkillResponses[2].SkillId);
        skills[2].SkillCodeCode.Should().Be(createdSkillResponses[2].SkillCodeCode);
        skills[2].SkillCodeLabel.Should().Be(createdSkillResponses[2].SkillCodeLabel);

        await _batchCreateSkillRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<SkillDTO>>(request.Skills);
        await _skillService.Received(1).BatchSkills(request.ProfileId, skillDTOs);
        _mapper.Received(1).Map<ICollection<CreateSkillResponse>>(Arg.Any<ICollection<SkillDTO>>());
    }
}