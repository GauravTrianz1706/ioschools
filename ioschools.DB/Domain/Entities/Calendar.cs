using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a calendar event entry.
    /// </summary>
    [Table("calendar")]
    public class Calendar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string title { get; set; } = string.Empty;

        public DateTime date { get; set; }

        public DateTime? end_date { get; set; }

        public string? description { get; set; }

        public int? schoolid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }
    }
}
