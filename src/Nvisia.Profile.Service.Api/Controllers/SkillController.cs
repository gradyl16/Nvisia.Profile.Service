using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Nvisia.Profile.Service.Api.Constants;
using Nvisia.Profile.Service.Api.Models.Skill;
using Nvisia.Profile.Service.Domain.Services;
using Nvisia.Profile.Service.DTO.Models;

namespace Nvisia.Profile.Service.Api.Controllers;

[ApiController]
[Route(ControllerConstants.Skill.BaseRoute)]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;
    private readonly IValidator<BatchSkillRequest> _batchCreateSkillRequestValidator;
    private readonly IMapper _mapper;

    public SkillController(ISkillService skillService,
        IValidator<BatchSkillRequest> batchCreateSkillRequestValidator,
        IMapper mapper)
    {
        _skillService = skillService ?? throw new ArgumentNullException(nameof(skillService));
        _batchCreateSkillRequestValidator = batchCreateSkillRequestValidator ?? throw new ArgumentNullException(nameof(batchCreateSkillRequestValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpPut(ControllerConstants.BatchRoute)]
    [ProducesResponseType(typeof(ICollection<CreateSkillResponse>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BatchSkills([FromBody] BatchSkillRequest request)
    {
        var validationResult = await _batchCreateSkillRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var skillDTOs = _mapper.Map<ICollection<SkillDTO>>(request.Skills);
        var createdSkills =
            await _skillService.BatchSkills(request.ProfileId, skillDTOs);
        
        if (createdSkills.Count is 0)
            return BadRequest();

        var createdSkillResponses = _mapper.Map<ICollection<CreateSkillResponse>>(createdSkills);
       
        return Accepted(createdSkillResponses);
    }
}