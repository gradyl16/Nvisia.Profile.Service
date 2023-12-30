using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nvisia.Profile.Service.WriteStore.Models;

/// <summary>
/// This is the Certification entity. A child of Profile. The table name will be 'certifications' by convention.
/// This would be used for professional certifications.
/// To have a different name, use the [Table()] annotation
/// The schema for the table is set in the DbContext.
/// </summary>
[Table("Certification")]
public class CertificationEntity
{
    /// <summary>
    /// This is the primary key of the entity.
    /// </summary>
    [Key]
    [Column("certification_id")]
    public int CertificationId { get; set; }

    [Column("title")]
    [Required]
    public string Title { get; set; } = null!;
    
    [Column("year")]
    [Required]
    public int Year { get; set; }

    /// <summary>
    /// This will generate a foreign key back to the profile. To override the default ProfileId, use the [ForeignKey("")]
    /// </summary>
    [Column("profile_id")]
    public int ProfileId { get; set; }
    
    [ForeignKey(nameof(ProfileId))]
    public ProfileEntity ProfileEntity { get; set; } = null!;
}