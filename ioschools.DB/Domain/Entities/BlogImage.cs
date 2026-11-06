using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents an image attached to a blog post.
    /// </summary>
    [Table("blog_image")]
    public class BlogImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string url { get; set; } = string.Empty;

        public long? blogid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(blogid))]
        public virtual Blog? blog { get; set; }
    }
}
