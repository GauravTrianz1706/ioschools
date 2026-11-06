using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a school class (form/grade).
    /// </summary>
    [Table("school_class")]
    public class SchoolClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public int schoolid { get; set; }

        public int? schoolyearid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        public virtual ICollection<ClassesStudentsAllocated> classes_students_allocateds { get; set; } = new List<ClassesStudentsAllocated>();

        public virtual ICollection<ClassesTeachersAllocated> classes_teachers_allocateds { get; set; } = new List<ClassesTeachersAllocated>();

        public virtual ICollection<ExamClass> exam_classes { get; set; } = new List<ExamClass>();

        public virtual ICollection<HomeworkClass> homework_classes { get; set; } = new List<HomeworkClass>();
    }
}
