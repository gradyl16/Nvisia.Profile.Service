using FluentValidation;
using Nvisia.Profile.Service.Api.Models.Profile;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.Validators.Errors;

namespace Nvisia.Profile.Service.Api.Validators.Profile;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    private readonly ITitleCodeService _titleCodeService;
    public UpdateProfileRequestValidator(ITitleCodeService titleCodeService)
    {
        _titleCodeService = titleCodeService ?? throw new ArgumentNullException(nameof(titleCodeService));

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .GreaterThan(0);
            
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(255);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(255);
        
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .MaximumLength(255)
            .EmailAddress();
        
        RuleFor(x => x.TitleCodeId)
            .NotEmpty()
            .GreaterThan(0)
            .MustAsync(async (id, _) => {
                var titleCodeDTOs = await _titleCodeService.GetTitleCodes();
                return titleCodeDTOs.Any(s => s.TitleCodeId == id);
            }).WithMessage(ValidatorErrors.PropertyDoesNotExistForId("Title Code"));
        
        RuleFor(x => x.YearsOfExperience)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}