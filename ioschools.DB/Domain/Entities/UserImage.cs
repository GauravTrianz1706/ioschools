using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a profile image for a user.
    /// </summary>
    [Table("user_image")]
    public class UserImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string url { get; set; } = string.Empty;

        public long userid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(userid))]
        public virtual User? user { get; set; }
    }
}
