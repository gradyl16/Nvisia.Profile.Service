using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nvisia.Profile.Service.WriteStore.Models;

/// <summary>
/// This is the skill entity. A child of Profile. The table name will be 'Skills' by convention.
/// This would be used for relevant skills that have been used/learned.
/// To have a different name, use the [Table()] annotation
/// The schema for the table is set in the DbContext.
/// </summary>
[Table("Skill")]
public class SkillEntity
{
    /// <summary>
    /// This is the primary key of the entity.
    /// </summary>
    [Key]
    [Column("skill_id")]
    public int SkillId { get; set; }
    
    [Column("skill_code_id")]
    public int SkillCodeId { get; set; }
    
    [ForeignKey(nameof(SkillCodeId))]
    public SkillCodeEntity SkillCode { get; set; } = null!;
    
    [Column("description")]
    [Required]
    public string Description { get; set; } = null!;
    
    [Column("sort_order")]
    [Required]
    public int SortOrder { get; set; }

    /// <summary>
    /// This will generate a foreign key back to the profile. To override the default ProfileId, use the [ForeignKey("")]
    /// </summary>
    [Column("profile_id")]
    public int ProfileId { get; set; }
    
    [ForeignKey(nameof(ProfileId))]
    public ProfileEntity ProfileEntity { get; set; } = null!;
}