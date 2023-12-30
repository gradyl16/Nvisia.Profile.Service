using AutoFixture;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using Nvisia.Profile.Service.Api.Mappers;
using Nvisia.Profile.Service.Api.Models.Certification;
using Nvisia.Profile.Service.Api.Models.Education;
using Nvisia.Profile.Service.Api.Models.Highlight;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Api.Models.SkillCode;
using Nvisia.Profile.Service.Api.Models.TitleCode;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.UnitTests.Api.Mappers;

public class ApiToDTOMapperProfileTest
{
    private static readonly Fixture Fixture = new();
    private readonly IMapper _mapper;

    public ApiToDTOMapperProfileTest()
    {
        var mapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<ApiToDTOMapperProfile>(); });
        _mapper = new Mapper(mapperConfig);
    }

    [Test]
    public void TestMapCreateProfileRequest_To_ProfileDTO()
    {
        // Arrange
        var request = Fixture.Build<CreateProfileRequest>()
            .With(x => x.FirstName, "FirstName")
            .With(x => x.LastName, "LastName")
            .With(x => x.EmailAddress, "EmailAddress")
            .With(x => x.TitleCodeId, 1)
            .With(x => x.YearsOfExperience,4)
            .Create();

        // Act
        var profileDTO = _mapper.Map<ProfileDTO>(request);

        // Assert
        profileDTO.ProfileId.Should().BeNull();
        profileDTO.Certifications.Should().BeEmpty();
        profileDTO.Educations.Should().BeEmpty();
        profileDTO.Highlights.Should().BeEmpty();
        profileDTO.Skills.Should().BeEmpty();

        request.FirstName.Should().Be(profileDTO.FirstName);
        request.LastName.Should().Be(profileDTO.LastName);
        request.EmailAddress.Should().Be(profileDTO.EmailAddress);
        request.TitleCodeId.Should().Be(profileDTO.TitleCodeId);
        request.YearsOfExperience.Should().Be(profileDTO.YearsOfExperience);
    }

    [Test]
    public void TestMap_ProfileDTO_To_GetProfileResponse()
    {
        var profileDTO = Fixture.Build<ProfileDTO>()
            .With(x => x.ProfileId, 1)
            .With(x => x.FirstName, "FirstName")
            .With(x => x.LastName, "LastName")
            .With(x => x.EmailAddress, "EmailAddress")
            .With(x => x.TitleCodeId, 1)
            .With(x => x.YearsOfExperience, 4)
            .With(x => x.AboutMe, "AboutMe")
            .With(x => x.TitleCode, new TitleCodeDTO { TitleCodeId = 1, Code = "Code", Description = "Description" })
            .With(x => x.Certifications,
                new List<CertificationDTO>
                    { new() { CertificationId = 1, Title = "Title", Year = 2022 } })
            .With(x => x.Educations,
                new List<EducationDTO>
                {
                    new()
                    {
                        EducationId = 1, SchoolName = "SchoolName", GraduationYear = 2022,
                        MajorDegreeName = "MajorDegreeName", MinorDegreeName = "MinorDegreeName"
                    }
                })
            .Create();

        // Act
        var response = _mapper.Map<GetProfileResponse>(profileDTO);

        // Assert
        response.FirstName.Should().Be(profileDTO.FirstName);
        response.LastName.Should().Be(profileDTO.LastName);
        response.EmailAddress.Should().Be(profileDTO.EmailAddress);
        response.YearsOfExperience.Should().Be(profileDTO.YearsOfExperience);
        response.AboutMe.Should().Be(profileDTO.AboutMe);

        response.TitleCode.TitleCodeId.Should().Be(profileDTO.TitleCode.TitleCodeId);
        response.TitleCode.Code.Should().Be(profileDTO.TitleCode.Code);
        response.TitleCode.Description.Should().Be(profileDTO.TitleCode.Description);

        var certifications = response.Certifications.ToList();
        certifications.Should().HaveCount(1);
        certifications[0].CertificationId.Should().Be(1);
        certifications[0].Title.Should().Be("Title");
        certifications[0].Year.Should().Be(2022);

        var educations = response.Educations.ToList();
        educations.Should().HaveCount(1);
        educations[0].EducationId.Should().Be(1);
        educations[0].SchoolName.Should().Be("SchoolName");
        educations[0].GraduationYear.Should().Be(2022);
        educations[0].MajorDegreeName.Should().Be("MajorDegreeName");
        educations[0].MinorDegreeName.Should().Be("MinorDegreeName");
    }

    [Test]
    public void TestMapUpdateProfileRequest_To_ProfileDTO()
    {
        // Arrange
        var request = Fixture.Build<UpdateProfileRequest>()
            .With(x => x.ProfileId, 1)
            .With(x => x.FirstName, "FirstName")
            .With(x => x.LastName, "LastName")
            .With(x => x.EmailAddress, "EmailAddress")
            .With(x => x.TitleCodeId, 1)
            .With(x => x.YearsOfExperience,4)
            .Create();

        // Act
        var profileDTO = _mapper.Map<ProfileDTO>(request);

        // Assert
        profileDTO.Certifications.Should().BeEmpty();
        profileDTO.Educations.Should().BeEmpty();
        profileDTO.Highlights.Should().BeEmpty();
        profileDTO.Skills.Should().BeEmpty();

        request.ProfileId.Should().Be(profileDTO.ProfileId);
        request.FirstName.Should().Be(profileDTO.FirstName);
        request.LastName.Should().Be(profileDTO.LastName);
        request.EmailAddress.Should().Be(profileDTO.EmailAddress);
        request.TitleCodeId.Should().Be(profileDTO.TitleCodeId);
        request.YearsOfExperience.Should().Be(profileDTO.YearsOfExperience);
    }

    [Test]
    public void TestMapCertificationRequest_To_CertificationDTO()
    {
        // Arrange
        var request = Fixture.Build<CertificationRequest>()
            .With(x => x.Title, "Title")
            .With(x => x.Year, 2022)
            .Create();

        // Act
        var certificationDTO = _mapper.Map<CertificationDTO>(request);

        // Assert
        certificationDTO.Title.Should().Be(request.Title);
        certificationDTO.Year.Should().Be(request.Year);
        certificationDTO.ProfileId.Should().Be(0);
    }
    
    [Test]
    public void TestMapCertificationDTO_To_CreateCertificationResponse()
    {
        // Arrange
        var certificationDTO = Fixture.Build<CertificationDTO>()
            .With(x => x.CertificationId, 1)
            .With(x => x.Title, "Title")
            .With(x => x.Year, 2022)
            .With(x => x.ProfileId, 1)
            .Create();

        // Act
        var response = _mapper.Map<CertificationDTO>(certificationDTO);

        // Assert
        response.CertificationId.Should().Be(certificationDTO.CertificationId);
        response.Title.Should().Be(certificationDTO.Title);
        response.Year.Should().Be(certificationDTO.Year);
        response.ProfileId.Should().Be(certificationDTO.ProfileId);
    }

    [Test]
    public void TestMapCreateEducation_To_EducationDTO()
    {
        // Arrange
        var request = Fixture.Build<EducationRequest>()
            .With(x => x.SchoolName, "SchoolName")
            .With(x => x.MajorDegreeName, "MajorDegreeName")
            .With(x => x.MinorDegreeName, "MinorDegreeName")
            .With(x => x.GraduationYear, 2022)
            .Create();

        // Act
        var educationDTO = _mapper.Map<EducationDTO>(request);

        // Assert
        educationDTO.SchoolName.Should().Be(request.SchoolName);
        educationDTO.MajorDegreeName.Should().Be(request.MajorDegreeName);
        educationDTO.MinorDegreeName.Should().Be(request.MinorDegreeName);
        educationDTO.GraduationYear.Should().Be(request.GraduationYear);
        educationDTO.ProfileId.Should().Be(0);
    }

   [Test]
    public void TestMapEducationDTO_To_CreateEducationResponse()
    {
        // Arrange
        var educationDTO = Fixture.Build<EducationDTO>()
            .With(x => x.EducationId, 1)
            .With(x => x.SchoolName, "SchoolName")
            .With(x => x.MajorDegreeName, "MajorDegreeName")
            .With(x => x.MinorDegreeName, "MinorDegreeName")
            .With(x => x.GraduationYear, 2022)
            .With(x => x.ProfileId, 1)
            .Create();

        // Act
        var response = _mapper.Map<EducationDTO>(educationDTO);

        // Assert
        response.EducationId.Should().Be(educationDTO.EducationId);
        response.MajorDegreeName.Should().Be(educationDTO.MajorDegreeName);
        response.MinorDegreeName.Should().Be(educationDTO.MinorDegreeName);
        response.GraduationYear.Should().Be(educationDTO.GraduationYear);
        response.ProfileId.Should().Be(educationDTO.ProfileId);
    }

    [Test]
    public void TestMapHighlightRequest_To_HighlightDTO()
    {
        // Arrange
        var request = Fixture.Build<HighlightRequest>()
            .With(x => x.Title, "Title")
            .With(x => x.Description, "Description")
            .Create();

        // Act
        var skillDTO = _mapper.Map<HighlightDTO>(request);

        // Assert
        skillDTO.Title.Should().Be(request.Title);
        skillDTO.Description.Should().Be(request.Description);
        skillDTO.ProfileId.Should().Be(0);
    }

   [Test]
    public void TestMapHighlightDTO_To_CreateHighlightResponse()
    {
        // Arrange
        var skillDTO = Fixture.Build<HighlightDTO>()
            .With(x => x.HighlightId, 1)
            .With(x => x.Title, "Title")
            .With(x => x.Description, "Description")
            .With(x => x.ProfileId, 1)
            .Create();

        // Act
        var response = _mapper.Map<CreateHighlightResponse>(skillDTO);

        // Assert
        response.HighlightId.Should().Be(skillDTO.HighlightId);
        response.Title.Should().Be(skillDTO.Title);
        response.Description.Should().Be(skillDTO.Description);
    }

    [Test]
    public void TestMapSkillCodeDTO_To_SkillCodeResponse()
    {
        // Arrange
        var skillCodeDTO = Fixture.Build<SkillCodeDTO>()
            .With(x => x.SkillCodeId,1)
            .With(x => x.Code, "Code")
            .With(x => x.Description, "Description")
            .Create();

        // Act
        var response = _mapper.Map<SkillCodeResponse>(skillCodeDTO);

        // Assert
        response.SkillCodeId.Should().Be(skillCodeDTO.SkillCodeId);
        response.Code.Should().Be(skillCodeDTO.Code);
        response.Label.Should().Be(skillCodeDTO.Description);
    }

    [Test]
    public void TestMapCreateSkill_To_SkillDTO()
    {
        // Arrange
        var request = Fixture.Build<SkillRequest>()
            .With(x => x.SkillCodeId, 1)
            .With(x => x.Description, "Description")
            .Create();

        // Act
        var skillDTO = _mapper.Map<SkillDTO>(request);

        // Assert
        skillDTO.SkillCodeId.Should().Be(request.SkillCodeId);
        skillDTO.Description.Should().Be(request.Description);
        skillDTO.ProfileId.Should().Be(0);
    }

   [Test]
    public void TestMapSkillDTO_To_CreateSkillResponse()
    {
        // Arrange
        var skillDTO = Fixture.Build<SkillDTO>()
            .With(x => x.SkillId, 1)
            .With(x => x.Description, "Description")
            .With(x => x.SkillCodeId, 1)
            .With(x => x.SkillCode, new SkillCodeDTO { SkillCodeId = 1, Code = "Code", Description = "Description" })
            .With(x => x.ProfileId, 1)
            .Create();

        // Act
        var response = _mapper.Map<CreateSkillResponse>(skillDTO);

        // Assert
        response.SkillId.Should().Be(skillDTO.SkillId);
        response.Description.Should().Be(skillDTO.Description);
        response.SkillCodeId.Should().Be(skillDTO.SkillCodeId);
        response.SkillCodeCode.Should().Be(skillDTO.SkillCode.Code);
        response.SkillCodeLabel.Should().Be(skillDTO.SkillCode.Description);
    }
    
    [Test]
    public void TestMapTitleCodeDTO_To_TitleCodeResponse()
    {
        // Arrange
        var titleCodeDTO = Fixture.Build<TitleCodeDTO>()
            .With(x => x.TitleCodeId,1)
            .With(x => x.Code, "Code")
            .With(x => x.Description, "Description")
            .Create();

        // Act
        var response = _mapper.Map<TitleCodeResponse>(titleCodeDTO);

        // Assert
        response.TitleCodeId.Should().Be(titleCodeDTO.TitleCodeId);
        response.Code.Should().Be(titleCodeDTO.Code);
        response.Description.Should().Be(titleCodeDTO.Description);
    }
}