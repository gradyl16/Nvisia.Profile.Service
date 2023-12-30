using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Nvisia.Profile.Service.Api.Formatters;
using Nvisia.Profile.Service.Api.Handlers;
using Nvisia.Profile.Service.Api.Mappers;
using Nvisia.Profile.Service.Api.Models.Certification;
using Nvisia.Profile.Service.Api.Models.Education;
using Nvisia.Profile.Service.Api.Models.Highlight;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Api.Validators.Certification;
using Nvisia.Profile.Service.Api.Validators.Education;
using Nvisia.Profile.Service.Api.Validators.Highlight;
using Nvisia.Profile.Service.Api.Validators.Profile;
using Nvisia.Profile.Service.Api.Validators.Skill;
using Serilog;
using Serilog.Formatting.Display;


namespace Nvisia.Profile.Service.Api;

public static class StartupExtensions
{
    public static IApplicationBuilder ConfigureApi(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
        return app;
    }

    public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }
    
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, config) => {
            var outputFormat = builder.Configuration.GetSection("OutputTemplate").Value!;
            config.ReadFrom.Configuration(context.Configuration);
            config.ReadFrom.Services(services);
            config.WriteTo.Console(new ParsedMessageFormatter(new MessageTemplateTextFormatter(outputFormat)));
            config.WriteTo.File(new ParsedMessageFormatter(new MessageTemplateTextFormatter(outputFormat)), "logs/log-.txt", rollingInterval: RollingInterval.Day);
        });
        return builder;
    }
    
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        
        // Disable default validator to give option for using FluentValidation
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        return services;
    }
    
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
    
    public static IServiceCollection AddDefaultCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy  =>
                {
                    policy.WithOrigins("http://*")
                        .AllowAnyOrigin()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithHeaders("Access-Control-Allow-Origin", "*");
                });
        });
        return services;
    }
    
    public static IServiceCollection AddApiExceptionHandlers(this IServiceCollection services)
    {
        services.AddExceptionHandler<DbUpdateExceptionHandler>();
        services.AddExceptionHandler<DefaultExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
    
    public static IServiceCollection AddApiMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApiToDTOMapperProfile));
        return services;
    }
    
    public static IServiceCollection AddApiValidation(this IServiceCollection services)
    {
        //Profiles
        services.AddScoped<IValidator<CreateProfileRequest>, CreateProfileRequestValidator>();
        services.AddScoped<IValidator<UpdateProfileRequest>, UpdateProfileRequestValidator>();
        services.AddScoped<IValidator<UpdateAboutMeRequest>, UpdateAboutMeRequestValidator>();
        
        //Certifications
        services.AddScoped<IValidator<BatchCertificationRequest>, BatchCertificationRequestValidator>();
        
        //Educations
        services.AddScoped<IValidator<BatchEducationRequest>, BatchEducationRequestValidator>();

        //Highlight
        services.AddScoped<IValidator<BatchHighlightRequest>, BatchHighlightRequestValidator>();

        //Skills
        services.AddScoped<IValidator<BatchSkillRequest>, BatchSkillRequestValidator>();
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Nvisia.Profile.Service", Version = "1.0.0"});
        });
        return services;
    }
}