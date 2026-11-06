using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents the association between a homework assignment and a school class.
    /// </summary>
    [Table("homework_class")]
    public class HomeworkClass
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long homeworkid { get; set; }

        public int classid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(homeworkid))]
        public virtual Homework? homework { get; set; }

        [ForeignKey(nameof(classid))]
        public virtual SchoolClass? school_class { get; set; }
    }
}
