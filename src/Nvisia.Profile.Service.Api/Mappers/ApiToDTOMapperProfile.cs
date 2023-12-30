using Nvisia.Profile.Service.Api.Models.Certification;
using Nvisia.Profile.Service.Api.Models.Education;
using Nvisia.Profile.Service.Api.Models.Highlight;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Api.Models.SkillCode;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Api.Models.TitleCode;
using Nvisia.Profile.Service.DTO.Models;
using AutoMapperProfile = AutoMapper.Profile;

namespace Nvisia.Profile.Service.Api.Mappers;

public class ApiToDTOMapperProfile : AutoMapperProfile
{
    public ApiToDTOMapperProfile()
    {
     
        //Profile
        CreateMap<ProfileDTO, GetProfileResponse>();
        
        CreateMap<CreateProfileRequest, ProfileDTO>();
        
        CreateMap<ProfileDTO, CreateProfileResponse>();

        CreateMap<UpdateProfileRequest, ProfileDTO>();

        //Certifications
        CreateMap<CertificationRequest, CertificationDTO>();
        
        CreateMap<CertificationDTO, CreateCertificationResponse>();
        
        //Educations
        CreateMap<EducationRequest, EducationDTO>();
        
        CreateMap<EducationDTO, CreateEducationResponse>();
        
        //Highlights
        CreateMap<HighlightRequest, HighlightDTO>();

        CreateMap<HighlightDTO, CreateHighlightResponse>();

        //SkillCode
        CreateMap<SkillCodeDTO, SkillCodeResponse>()
            .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Description));
        
        //Skills
        CreateMap<SkillRequest, SkillDTO>();

        CreateMap<SkillDTO, CreateSkillResponse>()
            .ForMember(d => d.SkillCodeLabel, opt => opt.MapFrom(src => src.SkillCode.Description));
        
        //TitleCode
        CreateMap<TitleCodeDTO, TitleCodeResponse>();
    }
}