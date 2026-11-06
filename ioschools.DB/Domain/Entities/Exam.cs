using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents an exam record.
    /// </summary>
    [Table("exam")]
    public class Exam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public int schoolid { get; set; }

        public int year { get; set; }

        public long creator { get; set; }

        public DateTime created { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        [ForeignKey(nameof(creator))]
        public virtual User? user { get; set; }

        public virtual ICollection<ExamClass> exam_classes { get; set; } = new List<ExamClass>();

        public virtual ICollection<ExamSubject> exam_subjects { get; set; } = new List<ExamSubject>();

        public virtual ICollection<ExamMark> exam_marks { get; set; } = new List<ExamMark>();
    }
}
