using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Context;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public class HighlightRepository : IHighlightRepository
{
    private readonly ApplicationDbContext _dbContext;

    public HighlightRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Insert multiple highlights
    /// </summary>
    /// <param name="highlights"></param>
    /// <returns>Collection of highlights</returns>
    public async Task<ICollection<HighlightEntity>> WriteHighlights(ICollection<HighlightEntity> highlights)
    {
        var existingHighlights = await GetExistingHighlights(highlights.First().ProfileId);

        var transaction = await _dbContext.Database.BeginTransactionAsync();

        _dbContext.Highlights.RemoveRange(existingHighlights);
        await _dbContext.SaveChangesAsync();

        await _dbContext.AddRangeAsync(highlights);
        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return highlights;
    }

    private async Task<ICollection<HighlightEntity>> GetExistingHighlights(int profileId) 
        => await _dbContext.Highlights.Where(x => x.ProfileId == profileId).ToListAsync();
}