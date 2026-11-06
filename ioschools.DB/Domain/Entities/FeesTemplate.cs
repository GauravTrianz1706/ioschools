using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a fees template for generating fee records.
    /// </summary>
    [Table("fees_template")]
    public class FeesTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long feestypeid { get; set; }

        public decimal amount { get; set; }

        public int? schoolyearid { get; set; }

        public int? schoolid { get; set; }

        // Navigation properties
        [ForeignKey(nameof(feestypeid))]
        public virtual FeesType? fees_type { get; set; }

        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }
    }
}
