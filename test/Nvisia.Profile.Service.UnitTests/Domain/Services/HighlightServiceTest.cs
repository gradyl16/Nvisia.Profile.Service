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

public class HighlightServiceTest
{
    private static readonly Fixture Fixture = new();
    private readonly IHighlightRepository _highlightRepository = Substitute.For<IHighlightRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly HighlightService _highlightService;

    public HighlightServiceTest()
    {
        _highlightService = new HighlightService(_highlightRepository, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _highlightRepository.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestBatchHighlights_FirstMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();

        _mapper.Map<ICollection<HighlightEntity>>(highlightDTOs).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _highlightService.BatchHighlights(profileId, highlightDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<HighlightEntity>>(highlightDTOs);
        await _highlightRepository.DidNotReceive()
            .WriteHighlights(Arg.Any<ICollection<HighlightEntity>>());
    }

    [Test]
    public async Task TestBatchHighlights_Repository_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();
        var highlightEntities =
            Fixture.Build<HighlightEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<HighlightEntity>>(highlightDTOs).Returns(highlightEntities);
        _highlightRepository.WriteHighlights(highlightEntities)
            .Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _highlightService.BatchHighlights(profileId, highlightDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<HighlightEntity>>(highlightDTOs);
        await _highlightRepository.Received(1).WriteHighlights(highlightEntities);
    }

    [Test]
    public async Task TestBatchHighlights_SecondMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();
        var highlightEntities =
            Fixture.Build<HighlightEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdHighlightEntities =
            Fixture.Build<HighlightEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<HighlightEntity>>(highlightDTOs).Returns(highlightEntities);
        _highlightRepository.WriteHighlights(highlightEntities).Returns(createdHighlightEntities);
        _mapper.Map<ICollection<HighlightDTO>>(createdHighlightEntities)
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _highlightService.BatchHighlights(profileId, highlightDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<HighlightEntity>>(highlightDTOs);
        await _highlightRepository.Received(1).WriteHighlights(highlightEntities);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(Arg.Any<ICollection<HighlightEntity>>());
    }

    [Test]
    public async Task TestBatchHighlights_Empty_Successful()
    {
        // Arrange
        const int profileId = 1;
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();
        var highlightEntities =
            Fixture.Build<HighlightEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdHighlightEntities = new List<HighlightEntity>();
        var createdHighlightDTOs = new List<HighlightDTO>();

        _mapper.Map<ICollection<HighlightEntity>>(highlightDTOs).Returns(highlightEntities);
        _highlightRepository.WriteHighlights(highlightEntities).Returns(createdHighlightEntities);
        _mapper.Map<ICollection<HighlightDTO>>(createdHighlightEntities).Returns(createdHighlightDTOs);
        
        // Act
        var result = await _highlightService.BatchHighlights(profileId, highlightDTOs);

        // Assert
        result.Should().BeEmpty();

        _mapper.Received(1).Map<ICollection<HighlightEntity>>(highlightDTOs);
        await _highlightRepository.Received(1).WriteHighlights(highlightEntities);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(createdHighlightEntities);
    }

    [Test]
    public async Task TestBatchHighlights_Populated_Successful()
    {
        // Arrange
        const int profileId = 1;
        var highlightDTOs = Fixture.Create<ICollection<HighlightDTO>>();
        var highlightEntities =
            Fixture.Build<HighlightEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdHighlightEntities =
            Fixture.Build<HighlightEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdHighlightDTOs = Fixture.CreateMany<HighlightDTO>(3).ToList();

        _mapper.Map<ICollection<HighlightEntity>>(highlightDTOs).Returns(highlightEntities);
        _highlightRepository.WriteHighlights(highlightEntities).Returns(createdHighlightEntities);
        _mapper.Map<ICollection<HighlightDTO>>(createdHighlightEntities).Returns(createdHighlightDTOs);

        // Act
        var result = await _highlightService.BatchHighlights(profileId, highlightDTOs);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
        var resultList = result.ToList();
        resultList[0].HighlightId.Should().NotBeNull();
        resultList[0].HighlightId.Should().Be(createdHighlightDTOs[0].HighlightId);
        resultList[0].Title.Should().Be(createdHighlightDTOs[0].Title);
        resultList[0].Description.Should().Be(createdHighlightDTOs[0].Description);
        resultList[0].ProfileId.Should().Be(createdHighlightDTOs[0].ProfileId);

        resultList[1].HighlightId.Should().NotBeNull();
        resultList[1].HighlightId.Should().Be(createdHighlightDTOs[1].HighlightId);
        resultList[1].Title.Should().Be(createdHighlightDTOs[1].Title);
        resultList[1].Description.Should().Be(createdHighlightDTOs[1].Description);
        resultList[1].ProfileId.Should().Be(createdHighlightDTOs[1].ProfileId);

        resultList[2].HighlightId.Should().NotBeNull();
        resultList[2].HighlightId.Should().Be(createdHighlightDTOs[2].HighlightId);
        resultList[2].Title.Should().Be(createdHighlightDTOs[2].Title);
        resultList[2].Description.Should().Be(createdHighlightDTOs[2].Description);
        resultList[2].ProfileId.Should().Be(createdHighlightDTOs[2].ProfileId);

        _mapper.Received(1).Map<ICollection<HighlightEntity>>(highlightDTOs);
        await _highlightRepository.Received(1).WriteHighlights(highlightEntities);
        _mapper.Received(1).Map<ICollection<HighlightDTO>>(createdHighlightEntities);
    }
}