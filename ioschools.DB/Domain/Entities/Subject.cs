using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a school subject.
    /// </summary>
    [Table("subject")]
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public int? schoolid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        public virtual ICollection<SubjectTeacher> subject_teachers { get; set; } = new List<SubjectTeacher>();

        public virtual ICollection<ClassesTeachersAllocated> classes_teachers_allocateds { get; set; } = new List<ClassesTeachersAllocated>();
    }
}
