using Microsoft.Extensions.DependencyInjection;
using Nvisia.Profile.Service.Domain.Mappers;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain;

public static class StartupExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services
            .AddCertificationServices()
            .AddProfileServices()
            .AddEducationServices()
            .AddHighlightServices()
            .AddSkillServices()
            .AddSkillCodeServices()
            .AddTitleCodeServices();
    }
    
    public static IServiceCollection AddDomainMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(EntityToDTOMapperProfile));
        return services;
    }
    
     private static IServiceCollection AddCertificationServices(this IServiceCollection services)
    {
        services.AddScoped<ICertificationService, CertificationService>();
        services.AddScoped<ICertificationRepository, CertificationRepository>();
        return services;
    }
    
    private static IServiceCollection AddProfileServices(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        return services;
    }
    
    private static IServiceCollection AddEducationServices(this IServiceCollection services)
    {
        services.AddScoped<IEducationService, EducationService>();
        services.AddScoped<IEducationRepository, EducationRepository>();
        return services;
    }
    
    private static IServiceCollection AddHighlightServices(this IServiceCollection services)
    {
        services.AddScoped<IHighlightService, HighlightService>();
        services.AddScoped<IHighlightRepository, HighlightRepository>();
        return services;
    }
    
    private static IServiceCollection AddSkillServices(this IServiceCollection services)
    {
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        return services;
    }
    
    private static IServiceCollection AddSkillCodeServices(this IServiceCollection services)
    {
        services.AddScoped<ISkillCodeService, SkillCodeService>();
        services.AddScoped<ISkillCodeRepository, SkillCodeRepository>();
        return services;
    }
    
    private static IServiceCollection AddTitleCodeServices(this IServiceCollection services)
    {
        services.AddScoped<ITitleCodeService, TitleCodeService>();
        services.AddScoped<ITitleCodeRepository, TitleCodeRepository>();
        return services;
    }
}