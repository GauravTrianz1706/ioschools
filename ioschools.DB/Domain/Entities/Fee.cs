using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a fee record for a student.
    /// </summary>
    [Table("fee")]
    public class Fee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long studentid { get; set; }

        public long feestypeid { get; set; }

        public decimal amount { get; set; }

        public DateTime duedate { get; set; }

        public string? status { get; set; }

        public DateTime? paiddate { get; set; }

        public int year { get; set; }

        // Navigation properties
        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(feestypeid))]
        public virtual FeesType? fees_type { get; set; }

        public virtual ICollection<FeesReminder> fees_reminders { get; set; } = new List<FeesReminder>();

        /// <summary>
        /// Returns the latest due date considering all reminders.
        /// </summary>
        [NotMapped]
        public DateTime duedateWithReminders
        {
            get
            {
                return fees_reminders
                    .Select(x => x.paymentdue)
                    .Union(new[] { duedate })
                    .OrderByDescending(y => y)
                    .First();
            }
        }
    }
}
