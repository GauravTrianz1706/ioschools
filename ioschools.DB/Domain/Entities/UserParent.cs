using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents additional parent/guardian profile information.
    /// </summary>
    [Table("user_parent")]
    public class UserParent
    {
        [Key]
        public long userid { get; set; }

        [MaxLength(200)]
        public string? occupation { get; set; }

        [MaxLength(200)]
        public string? employer { get; set; }

        [MaxLength(50)]
        public string? phone_office { get; set; }

        // Navigation properties
        [ForeignKey(nameof(userid))]
        public virtual User? user { get; set; }
    }
}
