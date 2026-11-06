using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a student's answer to a homework assignment.
    /// </summary>
    [Table("homework_answer")]
    public class HomeworkAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long homeworkid { get; set; }

        public long studentid { get; set; }

        public string? answer { get; set; }

        public DateTime? submitted { get; set; }

        // Navigation properties
        [ForeignKey(nameof(homeworkid))]
        public virtual Homework? homework { get; set; }

        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }
    }
}
