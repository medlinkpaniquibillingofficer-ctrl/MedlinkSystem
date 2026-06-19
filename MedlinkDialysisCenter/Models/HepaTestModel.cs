using System.ComponentModel.DataAnnotations;

namespace MedlinkDialysisCenter.Models
{
    public enum HepaResult
    {
        Positive,
        Negative
    }

    public class HepaTestModel{
        [Key]
        public int  HepaTestId  { get; set; }

        public int  PatientId   { get; set; }
        public Patient Patient  { get; set; } = null!;

        // Hepa B
        public bool         HepaBTested { get; set; }
        public HepaResult?  HepaBResult { get; set; }

        // Anti HBS
        public bool         AntiHBSTested { get; set; }
        public HepaResult?  AntiHBSResult { get; set; }

        // Hepa C
        public bool         HepaCTested { get; set; }
        public HepaResult?  HepaCResult { get; set; }

        public DateTime?    TestedAt    { get; set; }
        public string?      Remarks     { get; set; }
        public DateTime     CreatedAt   { get; set; } = DateTime.Now;
        public DateTime?    UpdatedAt   { get; set; }
    }
}