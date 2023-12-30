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

public class TitleCodeServiceTest
{
    private static readonly Fixture Fixture = new();
    private readonly ITitleCodeRepository _titleCodeRepository = Substitute.For<ITitleCodeRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    
    private readonly TitleCodeService _titleCodeService;

    public TitleCodeServiceTest()
    {
        _titleCodeService = new TitleCodeService(_titleCodeRepository, _mapper);
    }

    [SetUp]
    public void Setup()
    {
        _titleCodeRepository.ClearSubstitute();
        _mapper.ClearSubstitute();
    }
    
     [Test]
    public async Task TestGetTitleCodes_Repository_ThrowsException()
    {
        // Arrange
        _titleCodeRepository.GetTitleCodes().Throws(new Exception("Repository Exception"));

        // Act
        var action = () => _titleCodeService.GetTitleCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _titleCodeRepository.Received(1).GetTitleCodes();
        _mapper.DidNotReceive().Map<ICollection<TitleCodeDTO>>(Arg.Any<ICollection<TitleCodeEntity>>());
    }
    
    [Test]
    public async Task TestGetTitleCodes_Mapper_ThrowsException()
    {
        // Arrange
        var titleCodeEntities = Fixture.CreateMany<TitleCodeEntity>(3).ToList();
        
        _titleCodeRepository.GetTitleCodes().Returns(titleCodeEntities);
        _mapper.Map<ICollection<TitleCodeDTO>>(titleCodeEntities).Throws(new Exception("Mapper Exception"));
        
        // Act
        var action = () => _titleCodeService.GetTitleCodes();

        // Assert
        await action.Should().ThrowAsync<Exception>();

        await _titleCodeRepository.Received(1).GetTitleCodes();
        _mapper.Received(1).Map<ICollection<TitleCodeDTO>>(titleCodeEntities);
    }

    
    [Test]
    public async Task TestGetTitleCodes_ReturnsTitleCodes_Successful()
    {
        // Arrange
        var titleCodeEntities = Fixture.CreateMany<TitleCodeEntity>(3).ToList();
        var titleCodeDTOs = Fixture.CreateMany<TitleCodeDTO>(3).ToList();
        
        _titleCodeRepository.GetTitleCodes().Returns(titleCodeEntities);
        _mapper.Map<ICollection<TitleCodeDTO>>(titleCodeEntities).Returns(titleCodeDTOs);
        
        // Act
        var result = await _titleCodeService.GetTitleCodes();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(titleCodeDTOs);
        
        await _titleCodeRepository.Received(1).GetTitleCodes();
        _mapper.Received(1).Map<ICollection<TitleCodeDTO>>(titleCodeEntities);
    }

}