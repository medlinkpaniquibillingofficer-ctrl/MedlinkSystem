using System.ComponentModel.DataAnnotations;

namespace MedlinkDialysisCenter.Models
{
    public enum VaccineDose
    {
        First,
        Second,
        Booster
    }

    public class PatientVaccineModel
    {
        [Key]
        public int          PatientVaccineId    { get; set; }

        public int          PatientId           { get; set; }
        public Patient      Patient             { get; set; } = null!;

        [Required]
        public string       VaccineName         { get; set; } = string.Empty;

        public VaccineDose  Dose                { get; set; }

        [Required]
        public DateTime     DateGiven           { get; set; }

        public DateTime     CreatedAt           { get; set; } = DateTime.Now;
    }
}