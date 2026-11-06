using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents the assignment of a teacher to a subject for a class.
    /// </summary>
    [Table("subject_teacher")]
    public class SubjectTeacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long teacherid { get; set; }

        public long subjectid { get; set; }

        public int classid { get; set; }

        public int year { get; set; }

        // Navigation properties
        [ForeignKey(nameof(teacherid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(subjectid))]
        public virtual Subject? subject { get; set; }

        [ForeignKey(nameof(classid))]
        public virtual SchoolClass? school_class { get; set; }
    }
}
