using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents the assignment of a homework to a specific student.
    /// </summary>
    [Table("homework_student")]
    public class HomeworkStudent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long homeworkid { get; set; }

        public long studentid { get; set; }

        public int classid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(homeworkid))]
        public virtual Homework? homework { get; set; }

        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }
    }
}
