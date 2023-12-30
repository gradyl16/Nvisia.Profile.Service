using AutoMapper;
using Nvisia.Profile.Service.DTO.Models;
using Nvisia.Profile.Service.WriteStore.Models;
using Nvisia.Profile.Service.WriteStore.Repositories;

namespace Nvisia.Profile.Service.Domain.Services;

public class HighlightService : IHighlightService
{
    private readonly IHighlightRepository _highlightRepository;
    private readonly IMapper _mapper;

    public HighlightService(IHighlightRepository highlightRepository, IMapper mapper)
    {
        _highlightRepository = highlightRepository ?? throw new ArgumentNullException(nameof(highlightRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<ICollection<HighlightDTO>> BatchHighlights(int profileId, ICollection<HighlightDTO> highlights)
    {
        var highlightEntities = _mapper.Map<ICollection<HighlightEntity>>(highlights);
        highlightEntities.ToList().ForEach(x => x.ProfileId = profileId);
        var results = await _highlightRepository.WriteHighlights(highlightEntities);
        var highlightDTOs = _mapper.Map<ICollection<HighlightDTO>>(results);
        return highlightDTOs;
    }
}