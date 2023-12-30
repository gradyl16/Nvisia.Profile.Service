using FluentValidation;
using Nvisia.Profile.Service.Api.Models.Education;

namespace Nvisia.Profile.Service.Api.Validators.Education;

public class BatchEducationRequestValidator : AbstractValidator<BatchEducationRequest>
{
    public BatchEducationRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .GreaterThan(0);
        
        RuleForEach(x => x.Educations)
            .NotEmpty()
            .ChildRules(education =>
            {
                education.RuleFor(x => x.SchoolName)
                    .NotEmpty()
                    .MaximumLength(255);
                
                education.RuleFor(x => x.GraduationYear)
                    .NotEmpty()
                    .InclusiveBetween(1900, DateTime.Now.Year);
                
                education.RuleFor(x => x.MajorDegreeName)
                    .MaximumLength(255);
                
                education.When(x => x.MajorDegreeName is not null, () =>
                {
                    education.RuleFor(x => x.MajorDegreeName)
                        .Must(y => !string.IsNullOrWhiteSpace(y)); 
                });
                
                education.RuleFor(x => x.MinorDegreeName)
                    .MaximumLength(255);
                
                education.When(x => x.MinorDegreeName is not null, () =>
                {
                    education.RuleFor(x => x.MinorDegreeName)
                        .Must(y => !string.IsNullOrWhiteSpace(y)); 
                });
            });
    }
}