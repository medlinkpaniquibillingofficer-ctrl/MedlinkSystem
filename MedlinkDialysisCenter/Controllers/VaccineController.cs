using MedlinkDialysisCenter.Services;
using MedlinkDialysisCenter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedlinkDialysisCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VaccineController : Controller
    {
        private readonly VaccineService _vaccineService;
        private readonly PatientService _patientService;

        public VaccineController(VaccineService vaccineService, PatientService patientService)
        {
            _vaccineService = vaccineService;
            _patientService = patientService;
        }

        // GET: /Vaccine/Manage/5
        public async Task<IActionResult> Manage(int patientId)
        {
            var patient = await _patientService.GetById(patientId);
            if (patient == null) return NotFound();

            var vaccines = await _vaccineService.GetByPatientIdAsync(patientId);
            var vm = _vaccineService.MapToViewModel(vaccines, patient.PatientId, patient.PatientCode, patient.FullName);

            return View(vm);
        }

        // POST: /Vaccine/Manage
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(VaccineViewModel vm)
        {
            var patient = await _patientService.GetById(vm.PatientId);
            if (patient == null) return NotFound();

            await _vaccineService.SaveAsync(vm.PatientId, vm.Vaccines ?? new());
            return RedirectToAction("Details", "Patients", new { id = patient.PatientCode });
        }
    }
}