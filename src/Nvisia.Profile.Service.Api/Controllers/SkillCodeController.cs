using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nvisia.Profile.Service.Api.Constants;
using Nvisia.Profile.Service.Api.Errors;
using Nvisia.Profile.Service.Api.Models.SkillCode;
using Nvisia.Profile.Service.Domain.Services;

namespace Nvisia.Profile.Service.Api.Controllers;

[ApiController]
[Route(ControllerConstants.SkillCode.BaseRoute)]
public class SkillCodeController : ControllerBase
{
    private readonly ISkillCodeService _skillCodeService;
    private readonly IMapper _mapper;

    public SkillCodeController(ISkillCodeService skillCodeService, IMapper mapper)
    {
        _skillCodeService = skillCodeService ?? throw new ArgumentNullException(nameof(skillCodeService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ICollection<SkillCodeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSkillCodes()
    {
        var skillCodeDTOs = await _skillCodeService.GetSkillCodes();
        if (skillCodeDTOs.Count == 0)
            return NotFound(ControllerErrors.NotFound("Skill Codes"));
        
        var skillCodes = _mapper.Map<ICollection<SkillCodeResponse>>(skillCodeDTOs);
        return Ok(skillCodes);
    }
}