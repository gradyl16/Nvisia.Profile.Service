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

public class EducationServiceTest
{
    private static readonly Fixture Fixture = new();
    private readonly IEducationRepository _educationRepository = Substitute.For<IEducationRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly EducationService _educationService;

    public EducationServiceTest()
    {
        _educationService = new EducationService(_educationRepository, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _educationRepository.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestBatchEducations_FirstMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();

        _mapper.Map<ICollection<EducationEntity>>(educationDTOs).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _educationService.BatchEducations(profileId, educationDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<EducationEntity>>(educationDTOs);
        await _educationRepository.DidNotReceive()
            .WriteEducations(Arg.Any<ICollection<EducationEntity>>());
    }

    [Test]
    public async Task TestBatchEducations_Repository_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();
        var educationEntities =
            Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<EducationEntity>>(educationDTOs).Returns(educationEntities);
        _educationRepository.WriteEducations(educationEntities)
            .Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _educationService.BatchEducations(profileId, educationDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<EducationEntity>>(educationDTOs);
        await _educationRepository.Received(1).WriteEducations(educationEntities);
    }

    [Test]
    public async Task TestBatchEducations_SecondMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();
        var educationEntities =
            Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdEducationEntities =
            Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<EducationEntity>>(educationDTOs).Returns(educationEntities);
        _educationRepository.WriteEducations(educationEntities).Returns(createdEducationEntities);
        _mapper.Map<ICollection<EducationDTO>>(createdEducationEntities)
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _educationService.BatchEducations(profileId, educationDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<EducationEntity>>(educationDTOs);
        await _educationRepository.Received(1).WriteEducations(educationEntities);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(Arg.Any<ICollection<EducationEntity>>());
    }

    [Test]
    public async Task TestBatchEducations_Empty_Successful()
    {
        // Arrange
        const int profileId = 1;
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();
        var educationEntities =
            Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdEducationEntities = new List<EducationEntity>();
        var createdEducationDTOs = new List<EducationDTO>();

        _mapper.Map<ICollection<EducationEntity>>(educationDTOs).Returns(educationEntities);
        _educationRepository.WriteEducations(educationEntities).Returns(createdEducationEntities);
        _mapper.Map<ICollection<EducationDTO>>(createdEducationEntities).Returns(createdEducationDTOs);
        // Act
        var result = await _educationService.BatchEducations(profileId, educationDTOs);

        // Assert
        result.Should().BeEmpty();

        _mapper.Received(1).Map<ICollection<EducationEntity>>(educationDTOs);
        await _educationRepository.Received(1).WriteEducations(educationEntities);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(createdEducationEntities);
    }

    [Test]
    public async Task TestBatchEducations_Populated_Successful()
    {
        // Arrange
        const int profileId = 1;
        var educationDTOs = Fixture.Create<ICollection<EducationDTO>>();
        var educationEntities =
            Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdEducationEntities =
            Fixture.Build<EducationEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdEducationDTOs = Fixture.CreateMany<EducationDTO>(3).ToList();

        _mapper.Map<ICollection<EducationEntity>>(educationDTOs).Returns(educationEntities);
        _educationRepository.WriteEducations(educationEntities).Returns(createdEducationEntities);
        _mapper.Map<ICollection<EducationDTO>>(createdEducationEntities).Returns(createdEducationDTOs);

        // Act
        var result = await _educationService.BatchEducations(profileId, educationDTOs);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
        var resultList = result.ToList();
        resultList[0].EducationId.Should().NotBeNull();
        resultList[0].EducationId.Should().Be(createdEducationDTOs[0].EducationId);
        resultList[0].SchoolName.Should().Be(createdEducationDTOs[0].SchoolName);
        resultList[0].GraduationYear.Should().Be(createdEducationDTOs[0].GraduationYear);
        resultList[0].MajorDegreeName.Should().Be(createdEducationDTOs[0].MajorDegreeName);
        resultList[0].MinorDegreeName.Should().Be(createdEducationDTOs[0].MinorDegreeName);

        resultList[1].EducationId.Should().NotBeNull();
        resultList[1].EducationId.Should().Be(createdEducationDTOs[1].EducationId);
        resultList[1].SchoolName.Should().Be(createdEducationDTOs[1].SchoolName);
        resultList[1].GraduationYear.Should().Be(createdEducationDTOs[1].GraduationYear);
        resultList[1].MajorDegreeName.Should().Be(createdEducationDTOs[1].MajorDegreeName);
        resultList[1].MinorDegreeName.Should().Be(createdEducationDTOs[1].MinorDegreeName);

        resultList[2].EducationId.Should().NotBeNull();
        resultList[2].EducationId.Should().Be(createdEducationDTOs[2].EducationId);
        resultList[2].SchoolName.Should().Be(createdEducationDTOs[2].SchoolName);
        resultList[2].GraduationYear.Should().Be(createdEducationDTOs[2].GraduationYear);
        resultList[2].MajorDegreeName.Should().Be(createdEducationDTOs[2].MajorDegreeName);
        resultList[2].MinorDegreeName.Should().Be(createdEducationDTOs[2].MinorDegreeName);

        _mapper.Received(1).Map<ICollection<EducationEntity>>(educationDTOs);
        await _educationRepository.Received(1).WriteEducations(educationEntities);
        _mapper.Received(1).Map<ICollection<EducationDTO>>(createdEducationEntities);
    }
}