using MedlinkDialysisCenter.Services;
using MedlinkDialysisCenter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedlinkDialysisCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HepaTestController : Controller
    {
        private readonly HepaTestService _hepaTestService;
        private readonly PatientService _patientService;

        public HepaTestController(HepaTestService hepaTestService, PatientService patientService)
        {
            _hepaTestService = hepaTestService;
            _patientService = patientService;
        }

        // GET: /HepaTest/Manage/5
        public async Task<IActionResult> Manage(int patientId)
        {
            var patient = await _patientService.GetById(patientId);
            if (patient == null) return NotFound();

            var existing = await _hepaTestService.GetByPatientIdAsync(patientId);

            var vm = existing != null
                ? _hepaTestService.MapToViewModel(existing, patient.PatientCode, patient.FullName)
                : new HepaTestViewModel
                {
                    PatientId = patient.PatientId,
                    PatientCode = patient.PatientCode,
                    PatientName = patient.FullName
                };

            return View(vm);
        }

        // POST: /HepaTest/Manage
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(HepaTestViewModel vm)
        {
            var patient = await _patientService.GetById(vm.PatientId);
            if (patient == null) return NotFound();

            if (!vm.HepaBTested) ModelState.Remove(nameof(vm.HepaBResult));
            if (!vm.AntiHBSTested) ModelState.Remove(nameof(vm.AntiHBSResult));
            if (!vm.HepaCTested) ModelState.Remove(nameof(vm.HepaCResult));

            if (!ModelState.IsValid)
            {
                vm.PatientCode = patient.PatientCode;
                vm.PatientName = patient.FullName;
                return View(vm);
            }

            await _hepaTestService.SaveAsync(vm);
            return RedirectToAction("Details", "Patients", new { id = patient.PatientCode });
        }
    }
}