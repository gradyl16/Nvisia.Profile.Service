using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Context;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Repositories;

public class CertificationRepository : ICertificationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CertificationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Insert multiple certifications
    /// </summary>
    /// <param name="certifications"></param>
    /// <returns>Collection of certifications</returns>
    public async Task<ICollection<CertificationEntity>> WriteCertifications(ICollection<CertificationEntity> certifications)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();

        await DeleteCertificationsByProfileId(certifications.First().ProfileId);
        
        foreach (var certification in certifications)
        {
            await _dbContext.Certifications.AddAsync(certification);
            await _dbContext.SaveChangesAsync();
        }
        await transaction.CommitAsync();
        
        return certifications;
    }

    /// <summary>
    /// Delete all certifications related to a profile
    /// </summary>
    /// <param name="profileId"></param>
    /// <returns>true if already had no certifications or if it removed all existing ones</returns>
    public async Task<bool> DeleteCertificationsByProfileId(int profileId)
    {
        var existingCertifications = await GetExistingCertifications(profileId);
        if (existingCertifications.Count == 0)
            return true;
        
        _dbContext.Certifications.RemoveRange(existingCertifications);
        var numRemoved = await _dbContext.SaveChangesAsync();
        
        return numRemoved == existingCertifications.Count;
    }
    
    private async Task<ICollection<CertificationEntity>> GetExistingCertifications(int profileId) 
        => await _dbContext.Certifications.Where(x => x.ProfileId == profileId).ToListAsync();
}