using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public interface IHighlightRepository
{
    Task<ICollection<HighlightEntity>> WriteHighlights(ICollection<HighlightEntity> highlights);
}