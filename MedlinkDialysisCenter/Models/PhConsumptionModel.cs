using MedlinkDialysisCenter.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedlinkDialysisCenter.Models
{
    public class PhConsumptionModel
    {
        [Key]
        public int                  Id                  { get; set; }

        [Required]
        public int                  PatientId           { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Patient              Patient             { get; set; } = null!;

        [Required]
        public int                  YearCovered         { get; set; }

        [Required]
        public DateTime             SessionDate         { get; set; }

        [Required]
        [Range(1, 156)]
        public int                  SessionsConsumed    { get; set; } = 1;

        [Required]
        public PhConsumptionType    ConsumptionType     { get; set; }

        [MaxLength(200)]
        public string?              CenterName          { get; set; }

        [MaxLength(500)]
        public string?              Remarks             { get; set; }

        public DateTime             CreatedAt           { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string               CreatedBy           { get; set; } = string.Empty;
    }
}