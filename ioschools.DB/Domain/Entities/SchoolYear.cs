using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a school year (academic year level).
    /// </summary>
    [Table("school_year")]
    public class SchoolYear
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        public int schoolid { get; set; }

        public int year_level { get; set; }

        // Navigation properties
        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }
    }
}
