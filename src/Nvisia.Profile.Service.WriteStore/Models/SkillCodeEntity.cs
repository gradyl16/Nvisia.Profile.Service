using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nvisia.Profile.Service.WriteStore.Models;

/// <summary>
/// This is the SkillCode entity. A child of Profile. The table name will be 'Skill_Codes' by convention.
/// This would be used for the title of personal information.
/// To have a different name, use the [Table()] annotation
/// The schema for the table is set in the DbContext.
/// </summary>
[Table("SkillCode")]
public class SkillCodeEntity
{
    /// <summary>
    /// This is the primary key of the entity.
    /// </summary>
    [Key]
    [Column("skill_code_id")]
    public int SkillCodeId { get; set; }
    
    [Column("code")]
    [Required]
    public string Code { get; set; } = null!;
    
    [Column("description")]
    [Required]
    public string Description { get; set; } = null!;
}