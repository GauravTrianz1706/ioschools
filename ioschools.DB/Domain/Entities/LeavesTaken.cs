using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a leave taken record for a staff member.
    /// </summary>
    [Table("leaves_taken")]
    public class LeavesTaken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long userid { get; set; }

        public int leaveid { get; set; }

        public DateTime start_date { get; set; }

        public DateTime end_date { get; set; }

        public string? status { get; set; }

        public string? reason { get; set; }

        // Navigation properties
        [ForeignKey(nameof(userid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(leaveid))]
        public virtual Leave? leave { get; set; }
    }
}
