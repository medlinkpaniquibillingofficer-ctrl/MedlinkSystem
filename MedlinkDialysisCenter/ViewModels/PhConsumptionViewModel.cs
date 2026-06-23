using MedlinkDialysisCenter.Enums;
using System.ComponentModel.DataAnnotations;

namespace MedlinkDialysisCenter.ViewModels
{
    // ── Form ViewModel (Create / Edit) ──────────────────────────────────────
    public class PhConsumptionViewModel
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Session date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Session Date")]
        public DateTime SessionDate { get; set; } = DateTime.Today;

        [Required]
        [Range(1, 156, ErrorMessage = "Sessions consumed must be between 1 and 156.")]
        [Display(Name = "Sessions Consumed")]
        public int SessionsConsumed { get; set; } = 1;

        [Required]
        [Display(Name = "Type")]
        public PhConsumptionType ConsumptionType { get; set; }

        [MaxLength(200)]
        [Display(Name = "Center Name")]
        public string? CenterName { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        [Display(Name = "Year Covered")]
        public int YearCovered { get; set; } = DateTime.Now.Year;
    }

    // ── List Item ViewModel ─────────────────────────────────────────────────
    public class PhConsumptionListViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public int SessionsConsumed { get; set; }
        public PhConsumptionType ConsumptionType { get; set; }
        public string? CenterName { get; set; }
        public string? Remarks { get; set; }
        public int YearCovered { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // ── Patient Summary ViewModel ───────────────────────────────────────────
    public class PhSummaryViewModel
    {
        public const int MaxSessions = 156;

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public int YearCovered { get; set; }

        public int SessionsFromOtherCenter { get; set; }
        public int SessionsFromOwnCenter { get; set; }
        public int TotalUsed => SessionsFromOtherCenter + SessionsFromOwnCenter;
        public int Remaining => MaxSessions - TotalUsed;
        public double UsagePercent => Math.Round((double)TotalUsed / MaxSessions * 100, 1);

        public bool IsExhausted => Remaining <= 0;
        public bool IsWarning => Remaining is > 0 and <= 10;

        public List<PhConsumptionListViewModel> History { get; set; } = [];
    }

    // ── Index Page ViewModel (all patients summary) ─────────────────────────
    public class PhConsumptionIndexViewModel
    {
        public int YearFilter { get; set; } = DateTime.Now.Year;
        public List<PhSummaryViewModel> PatientSummaries { get; set; } = [];
    }
}