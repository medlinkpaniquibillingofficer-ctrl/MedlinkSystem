using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MedlinkDialysisCenter.Models
{
    public class PHRequirement
    {
        public int      Id                          { get; set; }

        [Required]
        public int      PatientId                   { get; set; }

        [MaxLength(50)]
        public string?  MemberCategory              { get; set; }
        public bool     HasCSF                      { get; set; }
        public bool     HasCF2                      { get; set; }
        public bool     HasMDR                      { get; set; }
        public bool     HasPhilhealthId             { get; set; }
        public bool     HasReceipt6Mos              { get; set; }       // Self-Employed
        public bool     HasCertMonthlyContrib       { get; set; }       // Employed Government
        public bool     HasSCId                     { get; set; }       // Senior Citizen
        public bool     HasCSFEmployerSig           { get; set; }       // Employed Government
        public bool     HasPDDRegistration          { get; set; }      // Indigent
        public bool     HasPhilhealthConsumption    { get; set; }

        [MaxLength(500)]
        public string?  Remarks                     { get; set; }

        public DateTime UpdatedAt                   { get; set; } = DateTime.Now;
        public DateTime CreatedAt                   { get; set; } = DateTime.Now;

        // Navigation — excluded from model binding validation
        [ValidateNever]
        public Patient  Patient                     { get; set; } = null!;

        // Computed: Complete only if all required docs for the category arAdd-Migration MigrationNamee submitted
        [NotMapped]
        public bool IsComplete{
            get{
                // Required for ALL categories
                if (!HasCSF ||
                    !HasCF2 ||
                    !HasMDR ||
                    !HasPhilhealthId ||
                    !HasPDDRegistration ||        // now required for all
                    !HasPhilhealthConsumption)
                    return false;

                return MemberCategory switch{
                    "Senior Citizen" => HasSCId,
                    "Indigent" => true,
                    "Self-Employed" => HasReceipt6Mos,
                    "Employed Government" => HasCertMonthlyContrib && HasCSFEmployerSig,
                    "Employed Private" => HasCertMonthlyContrib && HasCSFEmployerSig,
                    _ => false
                };
            }
        }
    }
}