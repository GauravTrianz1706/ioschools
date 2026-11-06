using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a type/category of fee.
    /// </summary>
    [Table("fees_type")]
    public class FeesType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public int? schoolid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }

        public virtual ICollection<Fee> fees { get; set; } = new List<Fee>();

        public virtual ICollection<FeesTemplate> fees_templates { get; set; } = new List<FeesTemplate>();
    }
}
