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

public class SkillServiceTest
{
    private static readonly Fixture Fixture = new();
    private readonly ISkillRepository _skillRepository = Substitute.For<ISkillRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly SkillService _skillService;

    public SkillServiceTest()
    {
        _skillService = new SkillService(_skillRepository, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _skillRepository.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestBatchSkills_FirstMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();

        _mapper.Map<ICollection<SkillEntity>>(skillDTOs).Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _skillService.BatchSkills(profileId, skillDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<SkillEntity>>(skillDTOs);
        await _skillRepository.DidNotReceive()
            .WriteSkills(Arg.Any<ICollection<SkillEntity>>());
    }

    [Test]
    public async Task TestBatchSkills_Repository_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();
        var skillEntities =
            Fixture.Build<SkillEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<SkillEntity>>(skillDTOs).Returns(skillEntities);
        _skillRepository.WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId)))
            .Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _skillService.BatchSkills(profileId, skillDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<SkillEntity>>(skillDTOs);
        await _skillRepository.Received(1).WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId)));
    }

    [Test]
    public async Task TestBatchSkills_SecondMapper_ThrowsException()
    {
        // Arrange
        const int profileId = 1;
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();
        var skillEntities =
            Fixture.Build<SkillEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdSkillEntities =
            Fixture.Build<SkillEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();

        _mapper.Map<ICollection<SkillEntity>>(skillDTOs).Returns(skillEntities);
        _skillRepository.WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId))).Returns(createdSkillEntities);
        _mapper.Map<ICollection<SkillDTO>>(createdSkillEntities)
            .Throws(new Exception("Mapper Exception"));

        // Act
        var action = () => _skillService.BatchSkills(profileId, skillDTOs);

        // Assert
        await action.Should().ThrowAsync<Exception>();

        _mapper.Received(1).Map<ICollection<SkillEntity>>(skillDTOs);
        await _skillRepository.Received(1).WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId)));
        _mapper.Received(1).Map<ICollection<SkillDTO>>(Arg.Any<ICollection<SkillEntity>>());
    }

    [Test]
    public async Task TestBatchSkills_Empty_Successful()
    {
        // Arrange
        const int profileId = 1;
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();
        var skillEntities =
            Fixture.Build<SkillEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdSkillEntities = new List<SkillEntity>();
        var createdSkillDTOs = new List<SkillDTO>();

        _mapper.Map<ICollection<SkillEntity>>(skillDTOs).Returns(skillEntities);
        _skillRepository.WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId))).Returns(createdSkillEntities);
        _mapper.Map<ICollection<SkillDTO>>(createdSkillEntities).Returns(createdSkillDTOs);
        
        // Act
        var result = await _skillService.BatchSkills(profileId, skillDTOs);

        // Assert
        result.Should().BeEmpty();

        _mapper.Received(1).Map<ICollection<SkillEntity>>(skillDTOs);
        await _skillRepository.Received(1).WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId)));
        _mapper.Received(1).Map<ICollection<SkillDTO>>(createdSkillEntities);
    }

    [Test]
    public async Task TestBatchSkills_Populated_Successful()
    {
        // Arrange
        const int profileId = 1;
        var skillDTOs = Fixture.Create<ICollection<SkillDTO>>();
        var skillEntities =
            Fixture.Build<SkillEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdSkillEntities =
            Fixture.Build<SkillEntity>().Without(x => x.ProfileEntity).CreateMany(3).ToList();
        var createdSkillDTOs = Fixture.CreateMany<SkillDTO>(3).ToList();

        _mapper.Map<ICollection<SkillEntity>>(skillDTOs).Returns(skillEntities);
        _skillRepository.WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId))).Returns(createdSkillEntities);
        _mapper.Map<ICollection<SkillDTO>>(createdSkillEntities).Returns(createdSkillDTOs);

        // Act
        var result = await _skillService.BatchSkills(profileId, skillDTOs);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(3);
        var resultList = result.ToList();
        resultList[0].SkillId.Should().NotBeNull();
        resultList[0].SkillId.Should().Be(createdSkillDTOs[0].SkillId);
        resultList[0].Description.Should().Be(createdSkillDTOs[0].Description);
        resultList[0].SkillCodeId.Should().Be(createdSkillDTOs[0].SkillCodeId);
        resultList[0].ProfileId.Should().Be(createdSkillDTOs[0].ProfileId);

        resultList[1].SkillId.Should().NotBeNull();
        resultList[1].SkillId.Should().Be(createdSkillDTOs[1].SkillId);
        resultList[1].Description.Should().Be(createdSkillDTOs[1].Description);
        resultList[1].SkillCodeId.Should().Be(createdSkillDTOs[1].SkillCodeId);
        resultList[1].ProfileId.Should().Be(createdSkillDTOs[1].ProfileId);

        resultList[2].SkillId.Should().NotBeNull();
        resultList[2].SkillId.Should().Be(createdSkillDTOs[2].SkillId);
        resultList[2].Description.Should().Be(createdSkillDTOs[2].Description);
        resultList[2].SkillCodeId.Should().Be(createdSkillDTOs[2].SkillCodeId);
        resultList[2].ProfileId.Should().Be(createdSkillDTOs[2].ProfileId);

        _mapper.Received(1).Map<ICollection<SkillEntity>>(skillDTOs);
        await _skillRepository.Received(1).WriteSkills(Arg.Is<ICollection<SkillEntity>>(skills => SkillEntityCollectionValidator(skills, profileId)));
        _mapper.Received(1).Map<ICollection<SkillDTO>>(createdSkillEntities);
    }

    private static bool SkillEntityCollectionValidator(ICollection<SkillEntity> skills, int profileId) 
    {
        int count = 0;
        return skills.All(skill => skill.ProfileId == profileId && skill.SortOrder == count++);
    }
}