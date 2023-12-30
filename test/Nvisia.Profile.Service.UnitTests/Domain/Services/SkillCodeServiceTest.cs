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

public class SkillCodeServiceTest
{
    private static readonly Fixture Fixture = new();
    private readonly ISkillCodeRepository _skillCodeRepository = Substitute.For<ISkillCodeRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    
    private readonly SkillCodeService _skillCodeService;

    public SkillCodeServiceTest()
    {
        _skillCodeService = new SkillCodeService(_skillCodeRepository, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _skillCodeRepository.ClearSubstitute();
        _mapper.ClearSubstitute();
    }

    [Test]
    public async Task TestGetSkillCodes_Repository_ThrowsException()
    {
        // Arrange
        _skillCodeRepository.GetSkillCodes().Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _skillCodeService.GetSkillCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _skillCodeRepository.Received(1).GetSkillCodes();
        _mapper.DidNotReceive().Map<ICollection<SkillCodeDTO>>(Arg.Any<ICollection<SkillCodeEntity>>());
    }
    
    [Test]
    public async Task TestGetSkillCodes_Mapper_ThrowsException()
    {
        // Arrange
        var skillCodeEntities = Fixture.CreateMany<SkillCodeEntity>(3).ToList();
        
        _skillCodeRepository.GetSkillCodes().Returns(skillCodeEntities);
        _mapper.Map<ICollection<SkillCodeDTO>>(skillCodeEntities).Throws(new Exception("Mapper Exception"));
        
        // Act
        var action = () => _skillCodeService.GetSkillCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _skillCodeRepository.Received(1).GetSkillCodes();
        _mapper.Received(1).Map<ICollection<SkillCodeDTO>>(skillCodeEntities);
    }

    
    [Test]
    public async Task TestGetSkillCodes_ReturnsSkillCodes_Successful()
    {
        // Arrange
        var skillCodeEntities = Fixture.CreateMany<SkillCodeEntity>(3).ToList();
        var skillCodeDTOs = Fixture.CreateMany<SkillCodeDTO>(3).ToList();
        
        _skillCodeRepository.GetSkillCodes().Returns(skillCodeEntities);
        _mapper.Map<ICollection<SkillCodeDTO>>(skillCodeEntities).Returns(skillCodeDTOs);
        
        // Act
        var result = await _skillCodeService.GetSkillCodes();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(skillCodeDTOs);
        
        await _skillCodeRepository.Received(1).GetSkillCodes();
        _mapper.Received(1).Map<ICollection<SkillCodeDTO>>(skillCodeEntities);
    }
}