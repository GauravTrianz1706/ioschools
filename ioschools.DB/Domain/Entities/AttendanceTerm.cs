using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents an attendance term configuration for a school.
    /// </summary>
    [Table("attendance_term")]
    public class AttendanceTerm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int schoolid { get; set; }

        public int termid { get; set; }

        public int year { get; set; }

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }
    }
}
