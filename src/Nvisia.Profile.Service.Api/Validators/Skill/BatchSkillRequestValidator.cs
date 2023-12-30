using FluentValidation;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.Validators.Errors;

namespace Nvisia.Profile.Service.Api.Validators.Skill;

public class BatchSkillRequestValidator : AbstractValidator<BatchSkillRequest>
{

    private readonly ISkillCodeService _skillCodeService;

    public BatchSkillRequestValidator(ISkillCodeService skillCodeService)
    {
        _skillCodeService = skillCodeService ?? throw new ArgumentNullException(nameof(skillCodeService));

        RuleFor(x => x.ProfileId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Skills)
            .NotEmpty()
            .Must(children => children.Count <= 6);
        
        RuleForEach(x => x.Skills)
            .NotEmpty()
            .ChildRules(skill =>
            {
                skill.RuleFor(x => x.SkillCodeId)
                    .NotEmpty()
                    .GreaterThan(0)
                    .MustAsync(async (id, _) => {
                        var skillCodeDTOs = await _skillCodeService.GetSkillCodes();
                        return skillCodeDTOs.Any(s => s.SkillCodeId == id);
                    }).WithMessage(ValidatorErrors.PropertyDoesNotExistForId("Skill Code"));

                skill.RuleFor(x => x.Description)
                    .NotEmpty()
                    .MaximumLength(200);
            });
    }
}