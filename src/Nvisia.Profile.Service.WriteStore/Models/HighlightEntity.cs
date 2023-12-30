using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nvisia.Profile.Service.WriteStore.Models;

/// <summary>
/// This is the Highlight entity. A child of Profile. The table name will be 'Highlights' by convention, so we changed it to 'Highlight'.
/// This would be used for jobs, projects, other Highlight. Start/End dates within the set need not be consecutive and can overlap
/// To have a different name, use the [Table()] annotation
/// The schema for the table is set in the DbContext.
/// </summary>
[Table("Highlight")]
public class HighlightEntity
{
    /// <summary>
    /// This is the primary key of the entity.
    /// </summary>
    [Key]
    [Column("highlight_id")]
    public int HighlightId { get; set; }

    [Column("title")]
    [Required]
    public string Title { get; set; } = null!;

    [Column("description")]
    [Required]
    public string Description { get; set; } = null!;

    /// <summary>
    /// This will generate a foreign key back to the profile. To override the default ProfileId, use the [ForeignKey("")]
    /// </summary>
    [Column("profile_id")]
    public int ProfileId { get; set; }
    
    [ForeignKey(nameof(ProfileId))]
    public ProfileEntity ProfileEntity { get; set; } = null!;
}