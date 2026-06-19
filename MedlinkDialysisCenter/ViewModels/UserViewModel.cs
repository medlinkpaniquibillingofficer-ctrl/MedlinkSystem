using System.ComponentModel.DataAnnotations;

namespace MedlinkDialysisCenter.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string   Email       { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string   Password    { get; set; } = string.Empty;

        [Required]
        public string   Role        { get; set; } = string.Empty; // "Admin" or "Nurse"
    }
}