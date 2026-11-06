using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a fee payment reminder.
    /// </summary>
    [Table("fees_reminder")]
    public class FeesReminder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long feeid { get; set; }

        public DateTime paymentdue { get; set; }

        public DateTime? sent { get; set; }

        // Navigation properties
        [ForeignKey(nameof(feeid))]
        public virtual Fee? fee { get; set; }
    }
}
