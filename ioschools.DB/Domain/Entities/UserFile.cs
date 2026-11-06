using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a file uploaded by a user.
    /// </summary>
    [Table("user_file")]
    public class UserFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string url { get; set; } = string.Empty;

        public long userid { get; set; }

        [MaxLength(255)]
        public string? filename { get; set; }

        // Navigation properties
        [ForeignKey(nameof(userid))]
        public virtual User? user { get; set; }
    }
}
