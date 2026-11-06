using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a homework assignment.
    /// </summary>
    [Table("homework")]
    public class Homework
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [Required]
        [MaxLength(500)]
        public string title { get; set; } = string.Empty;

        public string? description { get; set; }

        public long creator { get; set; }

        public DateTime created { get; set; }

        public DateTime? duedate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(creator))]
        public virtual User? user { get; set; }

        public virtual ICollection<HomeworkFile> homework_files { get; set; } = new List<HomeworkFile>();

        public virtual ICollection<HomeworkClass> homework_classes { get; set; } = new List<HomeworkClass>();

        public virtual ICollection<HomeworkAnswer> homework_answers { get; set; } = new List<HomeworkAnswer>();

        public virtual ICollection<HomeworkStudent> homework_students { get; set; } = new List<HomeworkStudent>();
    }
}
