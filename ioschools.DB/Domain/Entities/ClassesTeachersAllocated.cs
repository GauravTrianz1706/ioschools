using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a teacher-to-class allocation (timetable entry).
    /// </summary>
    [Table("classes_teachers_allocated")]
    public class ClassesTeachersAllocated
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long teacherid { get; set; }

        public int classid { get; set; }

        public int year { get; set; }

        public int day { get; set; }

        public TimeSpan? time_start { get; set; }

        public TimeSpan? time_end { get; set; }

        public int? schoolid { get; set; }

        public long? subjectid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(teacherid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(classid))]
        public virtual SchoolClass? school_class { get; set; }

        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        [ForeignKey(nameof(subjectid))]
        public virtual Subject? subject { get; set; }
    }
}
