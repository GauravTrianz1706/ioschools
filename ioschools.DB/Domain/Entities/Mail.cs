using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents an outgoing mail message record.
    /// </summary>
    [Table("mail")]
    public class Mail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        [MaxLength(500)]
        public string? toName { get; set; }

        [MaxLength(500)]
        public string? toEmail { get; set; }

        [MaxLength(500)]
        public string? subject { get; set; }

        public string? body { get; set; }

        [MaxLength(500)]
        public string? fromEmail { get; set; }

        [MaxLength(500)]
        public string? fromName { get; set; }

        public DateTime? created { get; set; }

        public string? status { get; set; }
    }
}
