using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Context;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public class EducationRepository : IEducationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EducationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Insert multiple educations
    /// </summary>
    /// <param name="educations"></param>
    /// <returns>Collection of educations</returns>
    public async Task<ICollection<EducationEntity>> WriteEducations(ICollection<EducationEntity> educations)
    {
        var existingEducations = await GetExistingEducations(educations.First().ProfileId);

        var transaction = await _dbContext.Database.BeginTransactionAsync();
        _dbContext.Educations.RemoveRange(existingEducations);
        await _dbContext.SaveChangesAsync();

        foreach (var education in educations)
        {
            await _dbContext.Educations.AddAsync(education);
            await _dbContext.SaveChangesAsync();
        }

        await transaction.CommitAsync();

        return educations;
    }
    
    private async Task<ICollection<EducationEntity>> GetExistingEducations(int profileId) 
        => await _dbContext.Educations.Where(x => x.ProfileId == profileId).ToListAsync();
}