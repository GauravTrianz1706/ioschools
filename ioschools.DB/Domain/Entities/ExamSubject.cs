using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a subject within an exam.
    /// </summary>
    [Table("exam_subject")]
    public class ExamSubject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long examid { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public long? subjectid { get; set; }

        public decimal? max_mark { get; set; }

        // Navigation properties
        [ForeignKey(nameof(examid))]
        public virtual Exam? exam { get; set; }

        [ForeignKey(nameof(subjectid))]
        public virtual Subject? subject { get; set; }

        public virtual ICollection<ExamMark> exam_marks { get; set; } = new List<ExamMark>();
    }
}
