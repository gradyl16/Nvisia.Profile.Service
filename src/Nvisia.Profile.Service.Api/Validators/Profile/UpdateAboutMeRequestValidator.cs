using FluentValidation;
using Nvisia.Profile.Service.Api.Models.Profile;

namespace Nvisia.Profile.Service.Api.Validators.Profile;

public class UpdateAboutMeRequestValidator : AbstractValidator<UpdateAboutMeRequest>
{
    public UpdateAboutMeRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .GreaterThan(0);
        
        RuleFor(x => x.AboutMe)
            .NotEmpty()
            .MaximumLength(255);
    }
}