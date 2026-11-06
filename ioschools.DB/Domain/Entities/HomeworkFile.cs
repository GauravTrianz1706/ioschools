using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a file attachment for a homework assignment.
    /// </summary>
    [Table("homework_file")]
    public class HomeworkFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string url { get; set; } = string.Empty;

        public long? homeworkid { get; set; }

        [MaxLength(255)]
        public string? filename { get; set; }

        public long size { get; set; }

        // Navigation properties
        [ForeignKey(nameof(homeworkid))]
        public virtual Homework? homework { get; set; }
    }
}
