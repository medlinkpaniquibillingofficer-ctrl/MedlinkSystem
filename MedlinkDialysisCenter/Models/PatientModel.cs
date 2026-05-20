using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedlinkDialysisCenter.Models
{
    public class Patient {
        public int PatientId { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? PhilhealthNo { get; set; }

        [MaxLength(20)]
        public string? ContactNo { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        [MaxLength(200)]
        public string? Diagnosis { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
    }
}