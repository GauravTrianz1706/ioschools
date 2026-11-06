using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents an exam template for reuse across exams.
    /// </summary>
    [Table("exam_template")]
    public class ExamTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public bool isprivate { get; set; }

        public long? creator { get; set; }

        // Navigation properties
        [ForeignKey(nameof(creator))]
        public virtual User? user { get; set; }

        public virtual ICollection<ExamTemplateSubject> exam_template_subjects { get; set; } = new List<ExamTemplateSubject>();
    }
}
