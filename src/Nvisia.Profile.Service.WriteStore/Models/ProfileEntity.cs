using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Nvisia.Profile.Service.WriteStore.Models;

/// <summary>
/// This is the profile entity. The top level of the hierarchy. The table name will be 'profiles' by convention.
/// To have a different name, use the [Table()] annotation
/// The schema for the table is set in the DbContext.
/// The index will enforce email uniqueness.
/// </summary>
[Table("Profile")]
[Index(nameof(EmailAddress), IsUnique = true)]
public class ProfileEntity
{
    /// <summary>
    /// This is the primary key of the entity.
    /// </summary>
    [Key]
    [Column("profile_id")]
    public int ProfileId { get; set; }

    /// <summary>
    /// This is an alternate key (Or Business Key), that will be used to search and identify the profile.
    /// </summary>
    [Column("email_address")]
    [Required]
    public string EmailAddress { get; set; } = null!;

    [Column("first_name")]
    [Required]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [Required]
    public string LastName { get; set; } = null!;

    [Column("title_code_id")]
    public int TitleCodeId { get; set; }
    
    [ForeignKey(nameof(TitleCodeId))]
    public TitleCodeEntity TitleCode { get; set; } = null!;
    
    [Column("years_of_experience")]
    [Required]
    public int YearsOfExperience { get; set; }

    [Column("about_me")]
    public string? AboutMe { get; set; }
    
    /// <summary>
    /// Below are the children of the profile
    /// </summary>
    public ICollection<CertificationEntity> Certifications { get; set; } = new List<CertificationEntity>();
    public ICollection<EducationEntity> Educations { get; set; } = new List<EducationEntity>();
    public ICollection<HighlightEntity> Highlights { get; set; } = new List<HighlightEntity>();
    public ICollection<SkillEntity> Skills { get; set; } = new List<SkillEntity>();
}