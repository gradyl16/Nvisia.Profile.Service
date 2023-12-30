using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;
using AutoMapperProfile = AutoMapper.Profile;

namespace Nvisia.Profile.Service.Domain.Mappers;

public class EntityToDTOMapperProfile : AutoMapperProfile
{
    public EntityToDTOMapperProfile()
    {
        CreateMap<ProfileEntity, ProfileDTO>()
            .ReverseMap();
        
        CreateMap<CertificationEntity, CertificationDTO>()
            .ReverseMap();

        CreateMap<EducationEntity, EducationDTO>()
            .ReverseMap();

        CreateMap<HighlightEntity, HighlightDTO>()
            .ReverseMap();

        CreateMap<SkillCodeEntity, SkillCodeDTO>()
            .ReverseMap();

        CreateMap<SkillEntity, SkillDTO>()
            .ReverseMap();
        
        CreateMap<TitleCodeEntity, TitleCodeDTO>()
            .ReverseMap();
    }
}