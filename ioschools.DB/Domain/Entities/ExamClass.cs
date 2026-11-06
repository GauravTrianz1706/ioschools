using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents the association between an exam and a school class.
    /// </summary>
    [Table("exam_class")]
    public class ExamClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public int classid { get; set; }

        public long examid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(examid))]
        public virtual Exam? exam { get; set; }

        [ForeignKey(nameof(classid))]
        public virtual SchoolClass? school_class { get; set; }
    }
}
