using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a student-to-class allocation.
    /// </summary>
    [Table("classes_students_allocated")]
    public class ClassesStudentsAllocated
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long studentid { get; set; }

        public int classid { get; set; }

        public int year { get; set; }

        // Navigation properties
        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(classid))]
        public virtual SchoolClass? school_class { get; set; }
    }
}
