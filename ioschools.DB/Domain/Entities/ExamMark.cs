using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a student's mark for an exam subject.
    /// </summary>
    [Table("exam_mark")]
    public class ExamMark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long examid { get; set; }

        public long studentid { get; set; }

        public long exam_subjectid { get; set; }

        public decimal? mark { get; set; }

        public string? grade { get; set; }

        // Navigation properties
        [ForeignKey(nameof(examid))]
        public virtual Exam? exam { get; set; }

        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(exam_subjectid))]
        public virtual ExamSubject? exam_subject { get; set; }
    }
}
