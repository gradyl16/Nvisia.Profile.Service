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
using Nvisia.Profile.Service.Api.Models.Highlight;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.UnitTests.Generators;

namespace Nvisia.Profile.Service.UnitTests.Api.Controllers;

public class HighlightControllerTest
{
	private static readonly Fixture Fixture = new();
	private readonly IHighlightService _highlightService = Substitute.For<IHighlightService>();
	private readonly IValidator<BatchHighlightRequest> _batchCreateHighlightRequestValidator =
        Substitute.For<IValidator<BatchHighlightRequest>>();
	private readonly IMapper _mapper = Substitute.For<IMapper>();
	private readonly HighlightController _controller;

	public HighlightControllerTest()
	{
		_controller = new HighlightController(_highlightService, _batchCreateHighlightRequestValidator, _mapper);
	}

	[SetUp]
	public void Setup()
	{
		_highlightService.ClearSubstitute();
		_batchCreateHighlightRequestValidator.ClearSubstitute();
		_mapper.ClearSubstitute();
	}

	[Test]
    public async Task TestBatchHighlights_ValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchHighlightRequest>();
        var validationResult = TestDataGenerator.GetFailedValidationResult();

        _batchCreateHighlightRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var response = await _controller.BatchHighlights(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        await _batchCreateHighlightRequestValidator.Received(1).ValidateAsync(request);
        _mapper.DidNotReceive().Map<ICollection<HighlightDTO>>(request);
    }

    [Test]
    public async Task TestBatchHighlights_FirstMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchHighlightRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _batchCreateHighlightRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<HighlightDTO>>(request.Highlights).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchHighlights(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateHighlightRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(request.Highlights);
        await _highlightService.DidNotReceive().BatchHighlights(Arg.Any<int>(), Arg.Any<ICollection<HighlightDTO>>());
    }

    [Test]
    public async Task TestBatchHighlights_Service_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchHighlightRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();

        _batchCreateHighlightRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<HighlightDTO>>(request.Highlights).Returns(highlightDTOs);
        _highlightService.BatchHighlights(request.ProfileId, highlightDTOs).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.BatchHighlights(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateHighlightRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(request.Highlights);
        await _highlightService.Received(1).BatchHighlights(request.ProfileId, highlightDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateHighlightResponse>>(Arg.Any<ICollection<HighlightDTO>>());
    }

    [Test]
    public async Task TestBatchHighlights_Service_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchHighlightRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();

        _batchCreateHighlightRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<HighlightDTO>>(request.Highlights).Returns(highlightDTOs);
        _highlightService.BatchHighlights(request.ProfileId, highlightDTOs)
            .Returns(new List<HighlightDTO>());

        // Act
        var response = await _controller.BatchHighlights(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();

        await _batchCreateHighlightRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(request.Highlights);
        await _highlightService.Received(1).BatchHighlights(request.ProfileId, highlightDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateHighlightResponse>>(Arg.Any<ICollection<HighlightDTO>>());
    }

    [Test]
    public async Task TestBatchHighlights_SecondMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchHighlightRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();
        var createdHighlights = Fixture.Create<ICollection<HighlightDTO>>();

        _batchCreateHighlightRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<HighlightDTO>>(request.Highlights).Returns(highlightDTOs);
        _highlightService.BatchHighlights(request.ProfileId, highlightDTOs).Returns(createdHighlights);
        _mapper.Map<ICollection<CreateHighlightResponse>>(Arg.Any<ICollection<HighlightDTO>>())
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchHighlights(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateHighlightRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(request.Highlights);
        await _highlightService.Received(1).BatchHighlights(request.ProfileId, highlightDTOs);
        _mapper.Received(1).Map<ICollection<CreateHighlightResponse>>(Arg.Any<ICollection<HighlightDTO>>());
    }

    [Test]
    public async Task TestBatchHighlights_Successful()
    {
        // Arrange
        var request = Fixture.Create<BatchHighlightRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();
        var createdHighlights = Fixture.Create<ICollection<HighlightDTO>>();
        var createdHighlightResponses = Fixture.CreateMany<CreateHighlightResponse>(3).ToList();

        _batchCreateHighlightRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<HighlightDTO>>(request.Highlights).Returns(highlightDTOs);
        _highlightService.BatchHighlights(request.ProfileId, highlightDTOs).Returns(createdHighlights);
        _mapper.Map<ICollection<CreateHighlightResponse>>(Arg.Any<ICollection<HighlightDTO>>())
            .Returns(createdHighlightResponses);

        // Act
        var response = await _controller.BatchHighlights(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AcceptedResult>();

        var value = ((AcceptedResult)response).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<List<CreateHighlightResponse>>();

        var highlights = value as List<CreateHighlightResponse>;
        highlights![0].HighlightId.Should().Be(createdHighlightResponses[0].HighlightId);
        highlights[0].Title.Should().Be(createdHighlightResponses[0].Title);
        highlights[0].Description.Should().Be(createdHighlightResponses[0].Description);

        highlights[1].HighlightId.Should().Be(createdHighlightResponses[1].HighlightId);
        highlights[1].Title.Should().Be(createdHighlightResponses[1].Title);
        highlights[1].Description.Should().Be(createdHighlightResponses[1].Description);

        highlights[2].HighlightId.Should().Be(createdHighlightResponses[2].HighlightId);
        highlights[2].Title.Should().Be(createdHighlightResponses[2].Title);
        highlights[2].Description.Should().Be(createdHighlightResponses[2].Description);

        await _batchCreateHighlightRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(request.Highlights);
        await _highlightService.Received(1).BatchHighlights(request.ProfileId, highlightDTOs);
        _mapper.Received(1).Map<ICollection<CreateHighlightResponse>>(Arg.Any<ICollection<HighlightDTO>>());
    }
}