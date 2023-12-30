using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Controllers;
using Nvisia.Profile.Service.Api.Errors;
using Nvisia.Profile.Service.Api.Models.TitleCode;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.UnitTests.Api.Controllers;

public class TitleCodeControllerTest
{
    private readonly ITitleCodeService _titleCodeService = Substitute.For<ITitleCodeService>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();

    private readonly TitleCodeController _controller;

    public TitleCodeControllerTest()
    {
        _controller = new TitleCodeController(_titleCodeService, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _titleCodeService.ClearSubstitute();
        _mapper.ClearSubstitute();
    }


    [Test]
    public async Task GetTitleCodes_TitleCodeService_ThrowsException()
    {
        // Arrange
        _titleCodeService.GetTitleCodes().Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.GetTitleCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _titleCodeService.Received(1).GetTitleCodes();
        _mapper.DidNotReceive().Map<ICollection<TitleCodeResponse>>(Arg.Any<ICollection<TitleCodeDTO>>());
    }

    [Test]
    public async Task GetTitleCodes_EmptyResult_ReturnsNotFound()
    {
        // Arrange
        _titleCodeService.GetTitleCodes().Returns(new List<TitleCodeDTO>());

        // Act
        var result = await _controller.GetTitleCodes();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var value = ((NotFoundObjectResult)result).Value as string;
        value.Should().Be(ControllerErrors.NotFound("Title Codes"));

        await _titleCodeService.Received(1).GetTitleCodes();
        _mapper.DidNotReceive().Map<ICollection<TitleCodeResponse>>(Arg.Any<ICollection<TitleCodeDTO>>());
    }

    [Test]
    public async Task GetTitleCodes_Mapper_ThrowsException()
    {
        // Arrange
        var titleCodeDTOs = new List<TitleCodeDTO> { new() { TitleCodeId = 1, Code = "Code", Description = "Description" } };
        _titleCodeService.GetTitleCodes().Returns(titleCodeDTOs);
        _mapper.Map<ICollection<TitleCodeResponse>>(titleCodeDTOs).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.GetTitleCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _titleCodeService.Received(1).GetTitleCodes();
        _mapper.Received(1).Map<ICollection<TitleCodeResponse>>(titleCodeDTOs);
    }

    [Test]
    public async Task GetTitleCodes_Successful()
    {
        // Arrange
        var titleCodeDTOs = new List<TitleCodeDTO> { new() { TitleCodeId = 1, Code = "Code", Description = "Description" } };
        var titleCodeResponses = new List<TitleCodeResponse> { new() { TitleCodeId = 1, Code = "Code", Description = "Description" } };
        _titleCodeService.GetTitleCodes().Returns(titleCodeDTOs);
        _mapper.Map<ICollection<TitleCodeResponse>>(titleCodeDTOs).Returns(titleCodeResponses);

        // Act
        var result = await _controller.GetTitleCodes();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        
        var value = ((OkObjectResult)result).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<List<TitleCodeResponse>>();
        
        var titleCodes = value as List<TitleCodeResponse>;
        titleCodes.Should().NotBeEmpty();
        titleCodes![0].TitleCodeId.Should().Be(titleCodeResponses[0].TitleCodeId);
        titleCodes[0].Code.Should().Be(titleCodeResponses[0].Code);
        titleCodes[0].Description.Should().Be(titleCodeResponses[0].Description);
        
        await _titleCodeService.Received(1).GetTitleCodes();
        _mapper.Received(1).Map<ICollection<TitleCodeResponse>>(titleCodeDTOs);
    }
}