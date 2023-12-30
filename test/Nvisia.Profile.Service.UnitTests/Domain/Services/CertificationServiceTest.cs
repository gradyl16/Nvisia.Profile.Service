using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.UnitTests.Domain.Services;

public class CertificationServiceTest
{
    private static readonly Fixture Fixture = new();
    private readonly ICertificationRepository _certificationRepository = Substitute.For<ICertificationRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly CertificationService _certificationService;

    public CertificationServiceTest()
    {
        _certificationService = new CertificationService(_certificationRepository, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _certificationRepository.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestBatchCertifications_Deletes_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.CreateMany<CertificationDTO>(0).ToList();

        _certificationRepository.DeleteCertificationsByProfileId(profileId).Throws(new Exception("Repository Exception"));
        // Act
        var action = () => _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _certificationRepository.Received(1).DeleteCertificationsByProfileId(profileId);
        _mapper.DidNotReceive().Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.DidNotReceive().WriteCertifications(Arg.Any<ICollection<CertificationEntity>>());
        _mapper.DidNotReceive().Map<ICollection<CertificationDTO>>(Arg.Any<ICollection<CertificationEntity>>());
    }

    [Test]
    public async Task TestBatchCertifications_Deletes_ReturnsEmptyList()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.CreateMany<CertificationDTO>(0).ToList();

        _certificationRepository.DeleteCertificationsByProfileId(profileId).Returns(true);
        // Act
        var result = await _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        await _certificationRepository.Received(1).DeleteCertificationsByProfileId(profileId);
        _mapper.DidNotReceive().Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.DidNotReceive().WriteCertifications(Arg.Any<ICollection<CertificationEntity>>());
        _mapper.DidNotReceive().Map<ICollection<CertificationDTO>>(Arg.Any<ICollection<CertificationEntity>>());
    }
    
    [Test]
    public async Task TestBatchCertifications_Deletes_ReturnsNull()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.CreateMany<CertificationDTO>(0).ToList();

        _certificationRepository.DeleteCertificationsByProfileId(profileId).Returns(false);
        // Act
        var result = await _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        result.Should().BeNull();

        await _certificationRepository.Received(1).DeleteCertificationsByProfileId(profileId);
        _mapper.DidNotReceive().Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.DidNotReceive().WriteCertifications(Arg.Any<ICollection<CertificationEntity>>());
        _mapper.DidNotReceive().Map<ICollection<CertificationDTO>>(Arg.Any<ICollection<CertificationEntity>>());
    }
    
    [Test]
    public async Task TestBatchCertifications_FirstMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();

        _mapper.Map<ICollection<CertificationEntity>>(certificationDTOs).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.DidNotReceive().WriteCertifications(Arg.Any<ICollection<CertificationEntity>>());
    }

    [Test]
    public async Task TestBatchCertifications_Repository_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();
        var certificationEntities =
            Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<CertificationEntity>>(certificationDTOs).Returns(certificationEntities);
        _certificationRepository.WriteCertifications(certificationEntities)
            .Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.Received(1).WriteCertifications(certificationEntities);
    }

    [Test]
    public async Task TestBatchCertifications_SecondMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();
        var certificationEntities =
            Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdCertificationEntities =
            Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<CertificationEntity>>(certificationDTOs).Returns(certificationEntities);
        _certificationRepository.WriteCertifications(certificationEntities).Returns(createdCertificationEntities);
        _mapper.Map<ICollection<CertificationDTO>>(createdCertificationEntities)
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.Received(1).WriteCertifications(certificationEntities);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(Arg.Any<ICollection<CertificationEntity>>());
    }

    [Test]
    public async Task TestBatchCertifications_Empty_Successful()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();
        var certificationEntities =
            Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdCertificationEntities = new List<CertificationEntity>();
        var createdCertificationDTOs = new List<CertificationDTO>();

        _mapper.Map<ICollection<CertificationEntity>>(certificationDTOs).Returns(certificationEntities);
        _certificationRepository.WriteCertifications(certificationEntities).Returns(createdCertificationEntities);
        _mapper.Map<ICollection<CertificationDTO>>(createdCertificationEntities).Returns(createdCertificationDTOs);
        // Act
        var result = await _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        result.Should().BeEmpty();

        _mapper.Received(1).Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.Received(1).WriteCertifications(certificationEntities);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(createdCertificationEntities);
    }

    [Test]
    public async Task TestBatchCertifications_Populated_Successful()
    {
        // Arrange
        const int profileId = 1;
        var certificationDTOs = Fixture.Create<ICollection<CertificationDTO>>();
        var certificationEntities =
            Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdCertificationEntities =
            Fixture.Build<CertificationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdCertificationDTOs = Fixture.CreateMany<CertificationDTO>(3).ToList();

        _mapper.Map<ICollection<CertificationEntity>>(certificationDTOs).Returns(certificationEntities);
        _certificationRepository.WriteCertifications(certificationEntities).Returns(createdCertificationEntities);
        _mapper.Map<ICollection<CertificationDTO>>(createdCertificationEntities).Returns(createdCertificationDTOs);

        // Act
        var result = await _certificationService.BatchCertifications(profileId, certificationDTOs);

        // Assert
        result.Should().NotBeNull();
        result!.Count.Should().Be(3);
        var resultList = result.ToList();
        resultList[0].CertificationId.Should().NotBeNull();
        resultList[0].CertificationId.Should().Be(createdCertificationDTOs[0].CertificationId);
        resultList[0].Title.Should().Be(createdCertificationDTOs[0].Title);
        resultList[0].Year.Should().Be(createdCertificationDTOs[0].Year);

        resultList[1].CertificationId.Should().NotBeNull();
        resultList[1].CertificationId.Should().Be(createdCertificationDTOs[1].CertificationId);
        resultList[1].Title.Should().Be(createdCertificationDTOs[1].Title);
        resultList[1].Year.Should().Be(createdCertificationDTOs[1].Year);

        resultList[2].CertificationId.Should().NotBeNull();
        resultList[2].CertificationId.Should().Be(createdCertificationDTOs[2].CertificationId);
        resultList[2].Title.Should().Be(createdCertificationDTOs[2].Title);
        resultList[2].Year.Should().Be(createdCertificationDTOs[2].Year);


        _mapper.Received(1).Map<ICollection<CertificationEntity>>(certificationDTOs);
        await _certificationRepository.Received(1).WriteCertifications(certificationEntities);
        _mapper.Received(1).Map<ICollection<CertificationDTO>>(createdCertificationEntities);
    }
}