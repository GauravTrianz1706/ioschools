using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a sibling relationship between two students.
    /// </summary>
    [Table("sibling")]
    public class Sibling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long siblingid { get; set; }

        public long otherid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(siblingid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(otherid))]
        public virtual User? user1 { get; set; }
    }
}
