using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ioschools.DB.Domain.Entities
{
    /// <summary>
    /// Represents a student registration/enrolment record.
    /// </summary>
    [Table("registration")]
    public class Registration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public long studentid { get; set; }

        [MaxLength(50)]
        public string? status { get; set; }

        public long? applicantid { get; set; }

        public int? schoolid { get; set; }

        public int? schoolyearid { get; set; }

        public long? reviewer { get; set; }

        public string? reviewMessage { get; set; }

        public int? enrollingYear { get; set; }

        public string? previous_school { get; set; }

        public string? previous_report { get; set; }

        public string? previous_class { get; set; }

        public string? leaving_reason { get; set; }

        public bool? isHandicap { get; set; }

        public bool? hasLearningProblem { get; set; }

        public string? disability_details { get; set; }

        public DateTime? admissionDate { get; set; }

        public DateTime? leftDate { get; set; }

        public string? test_academic { get; set; }

        public string? test_diligence { get; set; }

        public string? test_attendance { get; set; }

        public string? test_responsible { get; set; }

        public string? test_initiative { get; set; }

        public string? test_conduct { get; set; }

        public string? test_honesty { get; set; }

        public string? test_reliance { get; set; }

        public string? test_collaborate { get; set; }

        public string? test_appearance { get; set; }

        // Navigation properties
        [ForeignKey(nameof(studentid))]
        public virtual User? user { get; set; }

        [ForeignKey(nameof(schoolid))]
        public virtual School? school { get; set; }
    }
}
