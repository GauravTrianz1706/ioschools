using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents additional staff profile information.
    /// </summary>
    [Table("user_staff")]
    public class UserStaff
    {
        [Key]
        public long userid { get; set; }

        [MaxLength(200)]
        public string? department { get; set; }

        [MaxLength(200)]
        public string? position { get; set; }

        // Navigation properties
        [ForeignKey(nameof(userid))]
        public virtual User? user { get; set; }
    }
}
