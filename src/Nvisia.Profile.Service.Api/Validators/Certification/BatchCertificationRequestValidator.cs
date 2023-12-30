using FluentValidation;
using Nvisia.Profile.Service.Api.Models.Certification;

namespace Nvisia.Profile.Service.Api.Validators.Certification;

public class BatchCertificationRequestValidator : AbstractValidator<BatchCertificationRequest>
{
    public BatchCertificationRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .GreaterThan(0);
        
        RuleForEach(x => x.Certifications)
            .NotNull()
            .ChildRules(certification =>
            {
                certification.RuleFor(x => x.Title)
                    .NotEmpty()
                    .MaximumLength(255);

                certification.RuleFor(x => x.Year)
                    .NotEmpty()
                    .InclusiveBetween(1900, DateTime.Now.Year);
            });
    }
}