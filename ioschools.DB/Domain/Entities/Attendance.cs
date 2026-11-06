using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents an attendance record for a student.
    /// </summary>
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long studentid { get; set; }

        public DateTime date { get; set; }

        [MaxLength(50)]
        public string? status { get; set; }

        public int? classid { get; set; }

        public long? ecaid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(classid))]
        public virtual SchoolClass? school_class { get; set; }

        [ForeignKey(nameof(ecaid))]
        public virtual Eca? eca { get; set; }
    }
}
