using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a blog post or news entry.
    /// </summary>
    [Table("blog")]
    public class Blog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string title { get; set; } = string.Empty;

        public long creator { get; set; }

        public string? body { get; set; }

        public DateTime created { get; set; }

        public bool ispublic { get; set; }

        public bool emailed { get; set; }

        public byte? layoutMethod { get; set; }

        // Navigation properties
        [ForeignKey(nameof(creator))]
        public virtual User? user { get; set; }

        public virtual ICollection<BlogFile> blog_files { get; set; } = new List<BlogFile>();

        public virtual ICollection<BlogImage> blog_images { get; set; } = new List<BlogImage>();
    }
}
