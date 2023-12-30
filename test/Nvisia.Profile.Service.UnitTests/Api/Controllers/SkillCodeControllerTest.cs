using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Controllers;
using Nvisia.Profile.Service.Api.Errors;
using Nvisia.Profile.Service.Api.Models.SkillCode;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.UnitTests.Api.Controllers;

public class SkillCodeControllerTest
{
    private readonly ISkillCodeService _skillCodeService = Substitute.For<ISkillCodeService>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    private readonly SkillCodeController _controller;

    public SkillCodeControllerTest()
    {
        _controller = new SkillCodeController(_skillCodeService, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _skillCodeService.ClearSubstitute();
        _mapper.ClearSubstitute();
    }


    [Test]
    public async Task GetSkillCodes_SkillCodeService_ThrowsException()
    {
        // Arrange
        _skillCodeService.GetSkillCodes().Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.GetSkillCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _skillCodeService.Received(1).GetSkillCodes();
        _mapper.DidNotReceive().Map<ICollection<SkillCodeResponse>>(Arg.Any<ICollection<SkillCodeDTO>>());
    }

    [Test]
    public async Task GetSkillCodes_EmptyResult_ReturnsNotFound()
    {
        // Arrange
        _skillCodeService.GetSkillCodes().Returns(new List<SkillCodeDTO>());

        // Act
        var result = await _controller.GetSkillCodes();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var value = ((NotFoundObjectResult)result).Value as string;
        value.Should().Be(ControllerErrors.NotFound("Skill Codes"));

        await _skillCodeService.Received(1).GetSkillCodes();
        _mapper.DidNotReceive().Map<ICollection<SkillCodeResponse>>(Arg.Any<ICollection<SkillCodeDTO>>());
    }

    [Test]
    public async Task GetSkillCodes_Mapper_ThrowsException()
    {
        // Arrange
        var skillCodeDTOs = new List<SkillCodeDTO> { new() { SkillCodeId = 1, Code = "Code", Description = "Description" } };
        _skillCodeService.GetSkillCodes().Returns(skillCodeDTOs);
        _mapper.Map<ICollection<SkillCodeResponse>>(skillCodeDTOs).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.GetSkillCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _skillCodeService.Received(1).GetSkillCodes();
        _mapper.Received(1).Map<ICollection<SkillCodeResponse>>(skillCodeDTOs);
    }

    [Test]
    public async Task GetSkillCodes_Successful()
    {
        // Arrange
        var skillCodeDTOs = new List<SkillCodeDTO> { new() { SkillCodeId = 1, Code = "Code", Description = "Description" } };
        var skillCodeResponses = new List<SkillCodeResponse> { new() { SkillCodeId = 1, Code = "Code", Label = "Description" } };
        _skillCodeService.GetSkillCodes().Returns(skillCodeDTOs);
        _mapper.Map<ICollection<SkillCodeResponse>>(skillCodeDTOs).Returns(skillCodeResponses);

        // Act
        var result = await _controller.GetSkillCodes();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        
        var value = ((OkObjectResult)result).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<List<SkillCodeResponse>>();
        
        var skillCodes = value as List<SkillCodeResponse>;
        skillCodes.Should().NotBeEmpty();
        skillCodes![0].SkillCodeId.Should().Be(skillCodeResponses[0].SkillCodeId);
        skillCodes[0].Code.Should().Be(skillCodeResponses[0].Code);
        skillCodes[0].Label.Should().Be(skillCodeResponses[0].Label);
        
        await _skillCodeService.Received(1).GetSkillCodes();
        _mapper.Received(1).Map<ICollection<SkillCodeResponse>>(skillCodeDTOs);
    }
}