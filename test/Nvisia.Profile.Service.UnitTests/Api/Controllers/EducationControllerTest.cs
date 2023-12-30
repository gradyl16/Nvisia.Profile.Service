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
using Nvisia.Profile.Service.Api.Models.Education;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.UnitTests.Generators;

namespace Nvisia.Profile.Service.UnitTests.Api.Controllers;

public class EducationControllerTest
{
    private static readonly Fixture Fixture = new();

    private readonly IEducationService _educationService = Substitute.For<IEducationService>();

    private readonly IValidator<BatchEducationRequest> _batchCreateEducationRequestValidator =
        Substitute.For<IValidator<BatchEducationRequest>>();

    private readonly IMapper _mapper = Substitute.For<IMapper>();

    private readonly EducationController _controller;

    public EducationControllerTest()
    {
        _controller = new EducationController(_educationService, _batchCreateEducationRequestValidator, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _educationService.ClearSubstitute();
        _batchCreateEducationRequestValidator.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestBatchEducations_ValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchEducationRequest>();
        var validationResult = TestDataGenerator.GetFailedValidationResult();

        _batchCreateEducationRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var response = await _controller.BatchEducations(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        await _batchCreateEducationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.DidNotReceive().Map<ICollection<EducationDTO>>(request);
    }

    [Test]
    public async Task TestBatchEducations_FirstMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchEducationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _batchCreateEducationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<EducationDTO>>(request.Educations).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchEducations(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateEducationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(request.Educations);
        await _educationService.DidNotReceive().BatchEducations(Arg.Any<int>(), Arg.Any<ICollection<EducationDTO>>());
    }

    [Test]
    public async Task TestBatchEducations_Service_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchEducationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();

        _batchCreateEducationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<EducationDTO>>(request.Educations).Returns(educationDTOs);
        _educationService.BatchEducations(request.ProfileId, educationDTOs).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.BatchEducations(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateEducationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(request.Educations);
        await _educationService.Received(1).BatchEducations(request.ProfileId, educationDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateEducationResponse>>(Arg.Any<ICollection<EducationDTO>>());
    }

    [Test]
    public async Task TestBatchEducations_Service_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchEducationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();

        _batchCreateEducationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<EducationDTO>>(request.Educations).Returns(educationDTOs);
        _educationService.BatchEducations(request.ProfileId, educationDTOs)
            .Returns(new List<EducationDTO>());

        // Act
        var response = await _controller.BatchEducations(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();

        await _batchCreateEducationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(request.Educations);
        await _educationService.Received(1).BatchEducations(request.ProfileId, educationDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateEducationResponse>>(Arg.Any<ICollection<EducationDTO>>());
    }

    [Test]
    public async Task TestBatchEducations_SecondMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchEducationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();
        var createdEducations = Fixture.Create<ICollection<EducationDTO>>();

        _batchCreateEducationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<EducationDTO>>(request.Educations).Returns(educationDTOs);
        _educationService.BatchEducations(request.ProfileId, educationDTOs).Returns(createdEducations);
        _mapper.Map<ICollection<CreateEducationResponse>>(Arg.Any<ICollection<EducationDTO>>())
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchEducations(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateEducationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(request.Educations);
        await _educationService.Received(1).BatchEducations(request.ProfileId, educationDTOs);
        _mapper.Received(1).Map<ICollection<CreateEducationResponse>>(Arg.Any<ICollection<EducationDTO>>());
    }

    [Test]
    public async Task TestBatchEducations_Successful()
    {
        // Arrange
        var request = Fixture.Create<BatchEducationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();
        var createdEducations = Fixture.Create<ICollection<EducationDTO>>();
        var createdEducationResponses = Fixture.CreateMany<CreateEducationResponse>(3).ToList();

        _batchCreateEducationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<EducationDTO>>(request.Educations).Returns(educationDTOs);
        _educationService.BatchEducations(request.ProfileId, educationDTOs).Returns(createdEducations);
        _mapper.Map<ICollection<CreateEducationResponse>>(Arg.Any<ICollection<EducationDTO>>())
            .Returns(createdEducationResponses);

        // Act
        var response = await _controller.BatchEducations(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AcceptedResult>();

        var value = ((AcceptedResult)response).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<List<CreateEducationResponse>>();

        var educations = value as List<CreateEducationResponse>;
        educations![0].EducationId.Should().Be(createdEducationResponses[0].EducationId);
        educations[0].SchoolName.Should().Be(createdEducationResponses[0].SchoolName);
        educations[0].GraduationYear.Should().Be(createdEducationResponses[0].GraduationYear);
        educations[0].MajorDegreeName.Should().Be(createdEducationResponses[0].MajorDegreeName);
        educations[0].MinorDegreeName.Should().Be(createdEducationResponses[0].MinorDegreeName);

        educations[1].EducationId.Should().Be(createdEducationResponses[1].EducationId);
        educations[1].SchoolName.Should().Be(createdEducationResponses[1].SchoolName);
        educations[1].GraduationYear.Should().Be(createdEducationResponses[1].GraduationYear);
        educations[1].MajorDegreeName.Should().Be(createdEducationResponses[1].MajorDegreeName);
        educations[1].MinorDegreeName.Should().Be(createdEducationResponses[1].MinorDegreeName);

        educations[2].EducationId.Should().Be(createdEducationResponses[2].EducationId);
        educations[2].SchoolName.Should().Be(createdEducationResponses[2].SchoolName);
        educations[2].GraduationYear.Should().Be(createdEducationResponses[2].GraduationYear);
        educations[2].MajorDegreeName.Should().Be(createdEducationResponses[2].MajorDegreeName);
        educations[2].MinorDegreeName.Should().Be(createdEducationResponses[2].MinorDegreeName);

        await _batchCreateEducationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(request.Educations);
        await _educationService.Received(1).BatchEducations(request.ProfileId, educationDTOs);
        _mapper.Received(1).Map<ICollection<CreateEducationResponse>>(Arg.Any<ICollection<EducationDTO>>());
    }
}