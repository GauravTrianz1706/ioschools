using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents an Extra-Curricular Activity (ECA) entity.
    /// </summary>
    [Table("eca")]
    public class Eca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int schoolid { get; set; }

        [Required]
        [MaxLength(100)]
        public string name { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        public virtual ICollection<EcaStudent> eca_students { get; set; } = new List<EcaStudent>();

        public virtual ICollection<Attendance> attendances { get; set; } = new List<Attendance>();
    }
}
