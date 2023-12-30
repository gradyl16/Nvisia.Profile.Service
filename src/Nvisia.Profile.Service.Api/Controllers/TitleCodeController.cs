using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nvisia.Profile.Service.Api.Constants;
using Nvisia.Profile.Service.Api.Errors;
using Nvisia.Profile.Service.Api.Models.TitleCode;
using Nvisia.Profile.Service.Domain.Services;

namespace Nvisia.Profile.Service.Api.Controllers;

[ApiController]
[Route(ControllerConstants.TitleCode.BaseRoute)]
public class TitleCodeController : ControllerBase
{
    private readonly ITitleCodeService _titleCodeService;
    private readonly IMapper _mapper;

    public TitleCodeController(ITitleCodeService titleCodeService, IMapper mapper)
    {
        _titleCodeService = titleCodeService ?? throw new ArgumentNullException(nameof(titleCodeService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ICollection<TitleCodeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTitleCodes()
    {
        var titleCodeDTOs = await _titleCodeService.GetTitleCodes();
        if (titleCodeDTOs.Count == 0)
            return NotFound(ControllerErrors.NotFound("Title Codes"));
        
        var titleCodes = _mapper.Map<ICollection<TitleCodeResponse>>(titleCodeDTOs);
        return Ok(titleCodes);
    }
}