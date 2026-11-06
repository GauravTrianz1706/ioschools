using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a leave allocation for a staff member.
    /// </summary>
    [Table("leaves_allocated")]
    public class LeavesAllocated
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long userid { get; set; }

        public int leaveid { get; set; }

        public int year { get; set; }

        public decimal days { get; set; }

        // Navigation properties
        [ForeignKey(nameof(userid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(leaveid))]
        public virtual Leave? leave { get; set; }
    }
}
