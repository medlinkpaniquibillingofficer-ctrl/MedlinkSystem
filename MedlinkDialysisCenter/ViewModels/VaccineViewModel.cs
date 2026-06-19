using MedlinkDialysisCenter.Models;

namespace MedlinkDialysisCenter.ViewModels
{
    public class VaccineEntryViewModel
    {
        public int          PatientVaccineId    { get; set; }
        public string       VaccineName         { get; set; } = string.Empty;
        public VaccineDose  Dose                { get; set; }
        public DateTime?    DateGiven           { get; set; }
        public bool         IsDeleted           { get; set; }
    }

    public class VaccineViewModel
    {
        public int      PatientId   { get; set; }
        public string   PatientCode { get; set; } = string.Empty;
        public string   PatientName { get; set; } = string.Empty;
        public List<VaccineEntryViewModel> Vaccines { get; set; } = new();
    }
}