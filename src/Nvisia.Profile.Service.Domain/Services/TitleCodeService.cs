using AutoMapper;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain.Services;

public class TitleCodeService : ITitleCodeService
{
    private readonly ITitleCodeRepository _titleCodeRepository;
    private readonly IMapper _mapper;

    public TitleCodeService(ITitleCodeRepository titleCodeRepository, IMapper mapper)
    {
        _titleCodeRepository = titleCodeRepository ?? throw new ArgumentNullException(nameof(titleCodeRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ICollection<TitleCodeDTO>> GetTitleCodes()
    {
        var titleCodes = await _titleCodeRepository.GetTitleCodes();
        return _mapper.Map<ICollection<TitleCodeDTO>>(titleCodes);
    }
}