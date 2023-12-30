using AutoFixture;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Controllers;
using Nvisia.Profile.Service.Api.Models.Certification;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.UnitTests.Generators;

namespace Nvisia.Profile.Service.UnitTests.Api.Controllers;

public class CertificationControllerTest
{
    private static readonly Fixture Fixture = new();

    private readonly ICertificationService _certificationService = Substitute.For<ICertificationService>();

    private readonly IValidator<BatchCertificationRequest> _batchCreateCertificationRequestValidator =
        Substitute.For<IValidator<BatchCertificationRequest>>();

    private readonly IMapper _mapper = Substitute.For<IMapper>();

    private readonly CertificationController _controller;

    public CertificationControllerTest()
    {
        _controller =
            new CertificationController(_certificationService, _batchCreateCertificationRequestValidator, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _certificationService.ClearSubstitute();
        _batchCreateCertificationRequestValidator.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestBatchCertifications_ValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchCertificationRequest>();
        var validationResult = TestDataGenerator.GetFailedValidationResult();

        _batchCreateCertificationRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var response = await _controller.BatchCertifications(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestObjectResult>();

        await _batchCreateCertificationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.DidNotReceive().Map<ICollection<CertificationDTO>>(request);
    }

    [Test]
    public async Task TestBatchCertifications_FirstMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchCertificationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();

        _batchCreateCertificationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<CertificationDTO>>(request.Certifications).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchCertifications(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateCertificationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(request.Certifications);
        await _certificationService.DidNotReceive()
            .BatchCertifications(Arg.Any<int>(), Arg.Any<ICollection<CertificationDTO>>());
    }

    [Test]
    public async Task TestBatchCertifications_Service_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchCertificationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();

        _batchCreateCertificationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<CertificationDTO>>(request.Certifications).Returns(certificationDTOs);
        _certificationService.BatchCertifications(request.ProfileId, certificationDTOs).Throws(new Exception("Service Exception"));

        // Act
        var action = () => _controller.BatchCertifications(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateCertificationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(request.Certifications);
        await _certificationService.Received(1).BatchCertifications(request.ProfileId, certificationDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateCertificationResponse>>(Arg.Any<ICollection<CertificationDTO>>());
    }

    [Test]
    public async Task TestBatchCertifications_Service_ReturnsBadRequest()
    {
        // Arrange
        var request = Fixture.Create<BatchCertificationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();

        _batchCreateCertificationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<CertificationDTO>>(request.Certifications).Returns(certificationDTOs);
        _certificationService.BatchCertifications(request.ProfileId, certificationDTOs)
            .ReturnsNull();

        // Act
        var response = await _controller.BatchCertifications(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();

        await _batchCreateCertificationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(request.Certifications);
        await _certificationService.Received(1).BatchCertifications(request.ProfileId, certificationDTOs);
        _mapper.DidNotReceive().Map<ICollection<CreateCertificationResponse>>(Arg.Any<ICollection<CertificationDTO>>());
    }

    [Test]
    public async Task TestBatchCertifications_SecondMapping_ThrowsException()
    {
        // Arrange
        var request = Fixture.Create<BatchCertificationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();
        var createdCertifications = Fixture.Create<ICollection<CertificationDTO>>();

        _batchCreateCertificationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<CertificationDTO>>(request.Certifications).Returns(certificationDTOs);
        _certificationService.BatchCertifications(request.ProfileId, certificationDTOs)
            .Returns(createdCertifications);
        _mapper.Map<ICollection<CreateCertificationResponse>>(Arg.Any<ICollection<CertificationDTO>>())
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _controller.BatchCertifications(request);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _batchCreateCertificationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(request.Certifications);
        await _certificationService.Received(1).BatchCertifications(request.ProfileId, certificationDTOs);
        _mapper.Received(1).Map<ICollection<CreateCertificationResponse>>(Arg.Any<ICollection<CertificationDTO>>());
    }

    [Test]
    public async Task TestBatchCertifications_Successful()
    {
        // Arrange
        var request = Fixture.Create<BatchCertificationRequest>();
        var validationResult = TestDataGenerator.GetPassedValidationResult();
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();
        var createdCertifications = Fixture.Create<ICollection<CertificationDTO>>();
        var createdCertificationResponses = Fixture.CreateMany<CreateCertificationResponse>(3).ToList();

        _batchCreateCertificationRequestValidator.ValidateAsync(request).Returns(validationResult);
        _mapper.Map<ICollection<CertificationDTO>>(request.Certifications).Returns(certificationDTOs);
        _certificationService.BatchCertifications(request.ProfileId, certificationDTOs)
            .Returns(createdCertifications);
        _mapper.Map<ICollection<CreateCertificationResponse>>(Arg.Any<ICollection<CertificationDTO>>())
            .Returns(createdCertificationResponses);

        // Act
        var response = await _controller.BatchCertifications(request);

        // Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<AcceptedResult>();

        var value = ((AcceptedResult)response).Value;
        value.Should().NotBeNull();
        value.Should().BeOfType<List<CreateCertificationResponse>>();

        var certifications = value as List<CreateCertificationResponse>;
        certifications![0].CertificationId.Should().Be(createdCertificationResponses[0].CertificationId);
        certifications[0].Title.Should().Be(createdCertificationResponses[0].Title);

        certifications[1].CertificationId.Should().Be(createdCertificationResponses[1].CertificationId);
        certifications[1].Title.Should().Be(createdCertificationResponses[1].Title);

        certifications[2].CertificationId.Should().Be(createdCertificationResponses[2].CertificationId);
        certifications[2].Title.Should().Be(createdCertificationResponses[2].Title);


        await _batchCreateCertificationRequestValidator.Received(1).ValidateAsync(request);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(request.Certifications);
        await _certificationService.Received(1).BatchCertifications(request.ProfileId, certificationDTOs);
        _mapper.Received(1).Map<ICollection<CreateCertificationResponse>>(Arg.Any<ICollection<CertificationDTO>>());
    }
}