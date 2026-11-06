using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a staff employment period record.
    /// </summary>
    [Table("employment")]
    public class Employment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long userid { get; set; }

        public DateTime? start_date { get; set; }

        public DateTime? end_date { get; set; }

        [MaxLength(200)]
        public string? position { get; set; }

        // Navigation properties
        [ForeignKey(nameof(userid))]
        public virtual User? user { get; set; }
    }
}
