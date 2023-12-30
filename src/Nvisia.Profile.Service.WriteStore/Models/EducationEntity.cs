using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nvisia.Profile.Service.WriteStore.Models;

/// <summary>
/// This is the education entity. A child of Profile. The table name will be 'Educations' by convention, so we changed it to 'Education'.
/// This would be used for a degree or certicate from an educational entity
/// To have a different name, use the [Table()] annotation
/// The schema for the table is set in the DbContext.
/// </summary>
[Table("Education")]
public class EducationEntity
{
    /// <summary>
    /// This is the primary key of the entity.
    /// </summary>
    [Key]
    [Column("education_id")]
    public int EducationId { get; set; }

    [Column("school_name")]
    [Required]
    public string SchoolName { get; set; } = null!;
    
    [Column("graduation_year")]
    public int GraduationYear { get; set; }

    [Column("major_degree_name")]
    public string? MajorDegreeName { get; set; } 
    
    [Column("minor_degree_name")]
    public string? MinorDegreeName { get; set; } 

    /// <summary>
    /// This will generate a foreign key back to the profile. To override the default ProfileId, use the [ForeignKey("")]
    /// </summary>
    [Column("profile_id")]
    public int ProfileId { get; set; }
    
    [ForeignKey(nameof(ProfileId))]
    public ProfileEntity ProfileEntity { get; set; } = null!;

}