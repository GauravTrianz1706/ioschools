using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a grading rule within a grading method.
    /// </summary>
    [Table("grades_rule")]
    public class GradesRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int methodid { get; set; }

        [Required]
        [MaxLength(50)]
        public string grade { get; set; } = string.Empty;

        public decimal min_mark { get; set; }

        public decimal max_mark { get; set; }

        // Navigation properties
        [ForeignKey(nameof(methodid))]
        public virtual GradesMethod? grades_method { get; set; }
    }
}
