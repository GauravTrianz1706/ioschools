using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a leave type definition.
    /// </summary>
    [Table("leave")]
    public class Leave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public int? schoolid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        public virtual ICollection<LeavesAllocated> leaves_allocateds { get; set; } = new List<LeavesAllocated>();

        public virtual ICollection<LeavesTaken> leaves_takens { get; set; } = new List<LeavesTaken>();
    }
}
