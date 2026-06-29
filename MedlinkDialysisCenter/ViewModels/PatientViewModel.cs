namespace MedlinkDialysisCenter.Models
{
    public class PatientViewModel
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = "";
        public string PatientCode { get; set; } = "";
        public string FirstInitial { get; set; } = "";
        public string LastInitial { get; set; } = "";

        public string? PhilhealthNo { get; set; }
        public string? ContactNo { get; set; }
        public string? Diagnosis { get; set; }
        public string? Nephrologist { get; set; }

        public bool HasVaccines { get; set; }
        public HepaTestModel? HepaTest { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}