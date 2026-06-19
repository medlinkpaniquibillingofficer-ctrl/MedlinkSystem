using System.ComponentModel.DataAnnotations;

namespace MedlinkDialysisCenter.Enums
{
    public enum InventoryCategory
    {
        [Display(Name = "Medical Consumables")]
        MedicalConsumables,

        [Display(Name = "Medications & Solutions")]
        MedicationsAndSolutions,

        [Display(Name = "Equipment & Machines")]
        EquipmentAndMachines,

        [Display(Name = "Office & IT")]
        OfficeAndIT,

        [Display(Name = "Maintenance & Facility")]
        MaintenanceAndFacility,

        [Display(Name = "Linen & Patient Comfort")]
        LinenAndPatientComfort
    }
}