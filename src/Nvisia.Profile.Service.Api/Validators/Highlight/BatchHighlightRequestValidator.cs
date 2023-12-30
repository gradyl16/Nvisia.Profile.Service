using FluentValidation;
using Nvisia.Profile.Service.Api.Models.Highlight;

namespace Nvisia.Profile.Service.Api.Validators.Highlight;

public class BatchHighlightRequestValidator : AbstractValidator<BatchHighlightRequest>
{

    public BatchHighlightRequestValidator()
    {
        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Highlights)
            .NotEmpty()
            .Must(children => children.Count <= 3);
        
        RuleForEach(x => x.Highlights)
            .NotEmpty()
            .ChildRules(highlight =>
            {
                highlight.RuleFor(x => x.Title)
                    .NotEmpty()
                    .MaximumLength(50);

                highlight.RuleFor(x => x.Description)
                    .NotEmpty()
                    .MaximumLength(300);
            });
    }
}