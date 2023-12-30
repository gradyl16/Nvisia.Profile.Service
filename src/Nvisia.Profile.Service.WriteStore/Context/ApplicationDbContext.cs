using Microsoft.EntityFrameworkCore;
using Nvisia.Profile.Service.WriteStore.Models;

namespace Nvisia.Profile.Service.WriteStore.Context;

public class ApplicationDbContext : DbContext
{
    public virtual DbSet<ProfileEntity> Profiles { get; set; } = null!;
    
    public virtual DbSet<CertificationEntity> Certifications { get; set; } = null!;
    
    public virtual DbSet<EducationEntity> Educations { get; set; } = null!;
    
    public virtual DbSet<HighlightEntity> Highlights { get; set; } = null!;
    
    public virtual DbSet<SkillEntity> Skills { get; set; } = null!;
    
    public virtual DbSet<TitleCodeEntity> TitleCodes { get; set; } = null!;
    
    public virtual DbSet<SkillCodeEntity> SkillCodes { get; set; } = null!;
    
    
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    /// <summary>
    /// Used to mock this class for testing.
    /// </summary>
    public ApplicationDbContext()
    {
    }
}