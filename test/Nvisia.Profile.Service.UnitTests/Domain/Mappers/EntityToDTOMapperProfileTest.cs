using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.Domain.Mappers;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.UnitTests.Domain.Mappers;

public class EntityToDTOMapperProfileTest
{
    private static readonly Fixture Fixture = new();
    private readonly IMapper _mapper;

    public EntityToDTOMapperProfileTest()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EntityToDTOMapperProfile>();
        });
        _mapper = new Mapper(mapperConfig);
    }
    
    [Test]
    public void TestMapProfileEntity_To_ProfileDTO()
    {
        // Arrange
        var profileEntity = Fixture.Build<ProfileEntity>()
            .With(x => x.ProfileId, 1)
            .With(x => x.FirstName, "FirstName")
            .With(x => x.LastName, "LastName")
            .With(x => x.EmailAddress, "EmailAddress")
            .With(x => x.TitleCodeId, 1)
            .With(x => x.YearsOfExperience, 4)
            .With(x => x.AboutMe, "AboutMe")
            .With(x => x.TitleCode, new TitleCodeEntity())
            .With(x => x.Certifications, new List<CertificationEntity>())
            .With(x => x.Educations, new List<EducationEntity>())
            .With(x => x.Highlights, new List<HighlightEntity>())
            .With(x => x.Skills, new List<SkillEntity>())
            .Create();


        // Act
        var profileDTO = _mapper.Map<ProfileDTO>(profileEntity);

        // Assert
        profileDTO.ProfileId.Should().Be(profileEntity.ProfileId);
        profileDTO.FirstName.Should().Be(profileEntity.FirstName);
        profileDTO.LastName.Should().Be(profileEntity.LastName);
        profileDTO.EmailAddress.Should().Be(profileEntity.EmailAddress);
        profileDTO.TitleCodeId.Should().Be(profileEntity.TitleCodeId);
        profileDTO.YearsOfExperience.Should().Be(profileEntity.YearsOfExperience);
        profileDTO.AboutMe.Should().Be(profileEntity.AboutMe);
        profileDTO.TitleCode.Should().BeEquivalentTo(profileEntity.TitleCode);
        profileDTO.Certifications.Should().BeEquivalentTo(profileEntity.Certifications);
        profileDTO.Educations.Should().BeEquivalentTo(profileEntity.Educations);
        profileDTO.Highlights.Should().BeEquivalentTo(profileEntity.Highlights);
        profileDTO.Skills.Should().BeEquivalentTo(profileEntity.Skills);
    }
    
    [Test]
    public void TestMapProfileDTO_To_ProfileEntity()
    {
        // Arrange
        var profileDTO = Fixture.Build<ProfileDTO>()
            .With(x => x.ProfileId, 1)
            .With(x => x.FirstName, "FirstName")
            .With(x => x.LastName, "LastName")
            .With(x => x.EmailAddress, "EmailAddress")
            .With(x => x.TitleCodeId, 1)
            .With(x => x.YearsOfExperience, 4)
            .With(x => x.AboutMe, "AboutMe")
            .With(x => x.TitleCode, new TitleCodeDTO())
            .With(x => x.Certifications, new List<CertificationDTO>())
            .With(x => x.Educations, new List<EducationDTO>())
            .With(x => x.Highlights, new List<HighlightDTO>())
            .With(x => x.Skills, new List<SkillDTO>())
            .Create();


        // Act
        var profileEntity = _mapper.Map<ProfileEntity>(profileDTO);

        // Assert
        profileEntity.ProfileId.Should().Be(profileDTO.ProfileId);
        profileEntity.FirstName.Should().Be(profileDTO.FirstName);
        profileEntity.LastName.Should().Be(profileDTO.LastName);
        profileEntity.EmailAddress.Should().Be(profileDTO.EmailAddress);
        profileEntity.TitleCodeId.Should().Be(profileDTO.TitleCodeId);
        profileEntity.YearsOfExperience.Should().Be(profileDTO.YearsOfExperience);
        profileEntity.AboutMe.Should().Be(profileDTO.AboutMe);
        profileEntity.TitleCode.Should().BeEquivalentTo(profileDTO.TitleCode);
        profileEntity.Certifications.Should().BeEquivalentTo(profileDTO.Certifications);
        profileEntity.Educations.Should().BeEquivalentTo(profileDTO.Educations);
        profileEntity.Highlights.Should().BeEquivalentTo(profileDTO.Highlights);
        profileEntity.Skills.Should().BeEquivalentTo(profileDTO.Skills);
    }
    
    [Test]
    public void TestMapCertificationEntity_To_CertificationDTO()
    {
        // Arrange
        var certificationEntity = Fixture.Build<CertificationEntity>()
            .With(x => x.CertificationId, 1)
            .With(x => x.Title, "Title")
            .With(x => x.Year, 2022)
            .With(x => x.ProfileId, 1)
            .With(x => x.ProfileEntity, new ProfileEntity())
            .Create();

        // Act
        var certificationDTO = _mapper.Map<CertificationDTO>(certificationEntity);

        // Assert
        certificationDTO.CertificationId.Should().Be(certificationEntity.CertificationId);
        certificationDTO.Title.Should().Be(certificationEntity.Title);
        certificationDTO.Year.Should().Be(certificationEntity.Year);
        certificationDTO.ProfileId.Should().Be(certificationEntity.ProfileId);
    }
    
    [Test]
    public void TestMapCertificationDTO_To_CertificationEntity()
    {
        // Arrange
        var certificationDTO = Fixture.Build<CertificationDTO>()
            .With(x => x.CertificationId, 1)
            .With(x => x.Title, "Title")
            .With(x => x.Year, 2022)
            .With(x => x.ProfileId, 1)
            .Create();

        // Act
        var certificationEntity = _mapper.Map<CertificationEntity>(certificationDTO);

        // Assert
        certificationEntity.ProfileEntity.Should().BeNull();
        certificationEntity.CertificationId.Should().Be(certificationDTO.CertificationId);
        certificationEntity.Title.Should().Be(certificationDTO.Title);
        certificationEntity.Year.Should().Be(certificationDTO.Year);
        certificationEntity.ProfileId.Should().Be(certificationDTO.ProfileId);
    }
    
    [Test]
    public void TestMapEducationEntity_To_EducationDTO()
    {
        // Arrange
        var educationEntity = Fixture.Build<EducationEntity>()
            .With(x => x.SchoolName, "SchoolName")
            .With(x => x.MajorDegreeName, "MajorDegreeName")
            .With(x => x.MinorDegreeName, "MinorDegreeName")
            .With(x => x.GraduationYear, 2022)
            .With(x => x.ProfileId, 1)
            .With(x => x.ProfileEntity, new ProfileEntity())
            .Create();

        // Act
        var educationDTO = _mapper.Map<EducationDTO>(educationEntity);

        // Assert
        educationDTO.EducationId.Should().Be(educationEntity.EducationId);
        educationDTO.SchoolName.Should().Be(educationEntity.SchoolName);
        educationDTO.GraduationYear.Should().Be(educationEntity.GraduationYear);
        educationDTO.MajorDegreeName.Should().Be(educationEntity.MajorDegreeName);
        educationDTO.MinorDegreeName.Should().Be(educationEntity.MinorDegreeName);
        educationDTO.ProfileId.Should().Be(educationEntity.ProfileId);
    }
    
    [Test]
    public void TestMapEducationDTO_To_EducationEntity()
    {
        // Arrange
        var educationDTO = Fixture.Build<EducationDTO>()
            .With(x => x.SchoolName, "SchoolName")
            .With(x => x.MajorDegreeName, "MajorDegreeName")
            .With(x => x.MinorDegreeName, "MinorDegreeName")
            .With(x => x.GraduationYear, 2022)
            .With(x => x.ProfileId, 1)
            .Create();

        // Act
        var educationEntity = _mapper.Map<EducationEntity>(educationDTO);

        // Assert
        educationEntity.ProfileEntity.Should().BeNull();
        educationDTO.EducationId.Should().Be(educationEntity.EducationId);
        educationDTO.SchoolName.Should().Be(educationEntity.SchoolName);
        educationDTO.GraduationYear.Should().Be(educationEntity.GraduationYear);
        educationDTO.MajorDegreeName.Should().Be(educationEntity.MajorDegreeName);
        educationDTO.MinorDegreeName.Should().Be(educationEntity.MinorDegreeName);
        educationEntity.ProfileId.Should().Be(educationDTO.ProfileId);
    }

    [Test]
    public void TestMapHighlightEntity_To_HighlightDTO()
    {
        // Arrange
        var skillEntity = Fixture.Build<HighlightEntity>()
            .With(x => x.HighlightId, 1)
            .With(x => x.Title, "HighlightTitle")
            .With(x => x.Description, "HighlightDescription")
            .With(x => x.ProfileId, 1)
            .With(x => x.ProfileEntity, new ProfileEntity())
            .Create();

        // Act
        var skillDTO = _mapper.Map<HighlightDTO>(skillEntity);

        // Assert
        skillDTO.HighlightId.Should().Be(skillEntity.HighlightId);
        skillDTO.Title.Should().Be(skillEntity.Title);
        skillDTO.Description.Should().Be(skillEntity.Description);
        skillDTO.ProfileId.Should().Be(skillEntity.ProfileId);
    }
    
    [Test]
    public void TestMapHighlightDTO_To_HighlightEntity()
    {
        // Arrange
        var skillDTO = Fixture.Build<HighlightDTO>()
            .With(x => x.HighlightId, 1)
            .With(x => x.Description, "Title")
            .With(x => x.Description, "Description")
            .With(x => x.ProfileId, 1)
            .Create();

        // Act
        var skillEntity = _mapper.Map<HighlightEntity>(skillDTO);

        // Assert
        skillEntity.ProfileEntity.Should().BeNull();
        skillEntity.HighlightId.Should().Be(skillDTO.HighlightId);
        skillEntity.Title.Should().Be(skillDTO.Title);
        skillEntity.Description.Should().Be(skillDTO.Description);
        skillEntity.ProfileId.Should().Be(skillDTO.ProfileId);
    }

    [Test]
    public void TestMapSkillCodeEntity_To_SkillCodeDTO()
    {
        // Arrange
        var skillCodeEntity = Fixture.Build<SkillCodeEntity>()
            .With(x => x.SkillCodeId, 1)
            .With(x => x.Code, "Code")
            .With(x => x.Description, "Description")
            .Create();


        // Act
        var skillCodeDTO = _mapper.Map<SkillCodeDTO>(skillCodeEntity);

        // Assert
        skillCodeDTO.SkillCodeId.Should().Be(skillCodeEntity.SkillCodeId);
        skillCodeDTO.Code.Should().Be(skillCodeEntity.Code);
        skillCodeDTO.Description.Should().Be(skillCodeEntity.Description);
    }

    [Test]
    public void TestMapSkillEntity_To_SkillDTO()
    {
        // Arrange
        var skillEntity = Fixture.Build<SkillEntity>()
            .With(x => x.SkillId, 1)
            .With(x => x.SkillCodeId, 1)
            .With(x => x.SkillCode, new SkillCodeEntity())
            .With(x => x.Description, "SkillDescription")
            .With(x => x.SortOrder, 1)
            .With(x => x.ProfileId, 1)
            .With(x => x.ProfileEntity, new ProfileEntity())
            .Create();

        // Act
        var skillDTO = _mapper.Map<SkillDTO>(skillEntity);

        // Assert
        skillDTO.SkillId.Should().Be(skillEntity.SkillId);
        skillDTO.Description.Should().Be(skillEntity.Description);
        skillDTO.SkillCodeId.Should().Be(skillEntity.SkillCodeId);
        skillDTO.ProfileId.Should().Be(skillEntity.ProfileId);
    }
    
    [Test]
    public void TestMapSkillDTO_To_SkillEntity()
    {
        // Arrange
        var skillDTO = Fixture.Build<SkillDTO>()
            .With(x => x.SkillId, 1)
            .With(x => x.Description, "Description")
            .With(x => x.SkillCodeId, 0)
            .With(x => x.ProfileId, 2)
            .Create();

        // Act
        var skillEntity = _mapper.Map<SkillEntity>(skillDTO);

        // Assert
        skillEntity.ProfileEntity.Should().BeNull();
        skillEntity.SkillId.Should().Be(skillDTO.SkillId);
        skillEntity.Description.Should().Be(skillDTO.Description);
        skillEntity.SkillCodeId.Should().Be(skillDTO.SkillCodeId);
        skillEntity.ProfileId.Should().Be(skillDTO.ProfileId);
    }

    [Test]
    public void TestMapTitleCodeEntity_To_TitleCodeDTO()
    {
        // Arrange
        var titleCodeEntity = Fixture.Build<TitleCodeEntity>()
            .With(x => x.TitleCodeId, 1)
            .With(x => x.Code, "Code")
            .With(x => x.Description, "Description")
            .Create();


        // Act
        var titleCodeDTO = _mapper.Map<TitleCodeDTO>(titleCodeEntity);

        // Assert
        titleCodeDTO.TitleCodeId.Should().Be(titleCodeEntity.TitleCodeId);
        titleCodeDTO.Code.Should().Be(titleCodeEntity.Code);
        titleCodeDTO.Description.Should().Be(titleCodeEntity.Description);
    }
    
}