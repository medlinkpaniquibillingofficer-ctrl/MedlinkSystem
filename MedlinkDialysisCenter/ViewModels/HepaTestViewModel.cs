using MedlinkDialysisCenter.Models;

namespace MedlinkDialysisCenter.ViewModels
{
    public class HepaTestViewModel
    {
        public int      PatientId   { get; set; }
        public string   PatientCode { get; set; } = string.Empty;
        public string   PatientName { get; set; } = string.Empty;

        // Hepa B
        public bool         HepaBTested { get; set; }
        public HepaResult?  HepaBResult { get; set; }

        // Anti HBS
        public bool         AntiHBSTested { get; set; }
        public HepaResult?  AntiHBSResult { get; set; }

        // Hepa C
        public bool         HepaCTested { get; set; }
        public HepaResult?  HepaCResult { get; set; }
        public string?      Remarks     { get; set; }
    }
}