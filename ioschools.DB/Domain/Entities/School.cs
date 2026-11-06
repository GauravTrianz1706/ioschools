using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a school entity.
    /// </summary>
    [Table("school")]
    public class School
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        [MaxLength(200)]
        public string name { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<SchoolClass> school_classes { get; set; } = new List<SchoolClass>();

        public virtual ICollection<SchoolTerm> school_terms { get; set; } = new List<SchoolTerm>();

        public virtual ICollection<SchoolYear> school_years { get; set; } = new List<SchoolYear>();

        public virtual ICollection<Eca> ecas { get; set; } = new List<Eca>();

        public virtual ICollection<User> users { get; set; } = new List<User>();
    }
}
