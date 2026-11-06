using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a student discipline record.
    /// </summary>
    [Table("students_discipline")]
    public class StudentsDiscipline
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long studentid { get; set; }

        public long? teacherid { get; set; }

        public string? description { get; set; }

        public int points { get; set; }

        public DateTime created { get; set; }

        // Navigation properties
        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(teacherid))]
        public virtual User? user1 { get; set; }
    }
}
