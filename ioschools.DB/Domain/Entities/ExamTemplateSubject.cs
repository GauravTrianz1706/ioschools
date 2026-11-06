using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a subject within an exam template.
    /// </summary>
    [Table("exam_template_subject")]
    public class ExamTemplateSubject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int templateid { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public long? subjectid { get; set; }

        public decimal? max_mark { get; set; }

        // Navigation properties
        [ForeignKey(nameof(templateid))]
        public virtual ExamTemplate? exam_template { get; set; }

        [ForeignKey(nameof(subjectid))]
        public virtual Subject? subject { get; set; }
    }
}
