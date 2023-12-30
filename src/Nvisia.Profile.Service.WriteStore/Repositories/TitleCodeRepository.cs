using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Context;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public class TitleCodeRepository : ITitleCodeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TitleCodeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns All titles, no children, READ-ONLY entities
    /// </summary>
    /// <returns>List of Titles</returns>
    public async Task<ICollection<TitleCodeEntity>> GetTitleCodes()
        => await _dbContext.TitleCodes.AsNoTracking().ToListAsync();
}