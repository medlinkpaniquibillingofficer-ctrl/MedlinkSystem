using MedlinkDialysisCenter.Models;
using MedlinkDialysisCenter.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedlinkDialysisCenter.Controllers
{
    [Authorize(Roles = "Admin,Nurse")]
    public class PatientsController : Controller
    {
        private readonly PatientService _patientService;

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        public async Task<IActionResult> Index(int page = 1, string? search = null){
            const int pageSize = 7;
            var result = await _patientService.GetActivePatients(page, pageSize, search);
            return View(result);
        }

        public async Task<IActionResult> Details(string id)
        {
            var patient = await _patientService.GetByCode(id);
            if (patient == null) return NotFound();
            return View(patient);
        } 

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (!ModelState.IsValid) return View(patient);
            await _patientService.CreatePatient(patient);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientService.GetById(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.PatientId) return NotFound();
            if (!ModelState.IsValid) return View(patient);
            await _patientService.UpdatePatient(patient);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientService.GetById(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _patientService.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Cancelled() =>
            View(await _patientService.GetDeletedPatients());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id){
            await _patientService.Restore(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Celebrants() =>
            View(_patientService.GetCelebrantsThisMonth());

        public IActionResult ExportToExcel(){
            var bytes = _patientService.ExportToExcel();
            var fileName = $"Patients_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}