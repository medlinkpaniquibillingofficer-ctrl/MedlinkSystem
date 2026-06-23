using MedlinkDialysisCenter.Enums;
using MedlinkDialysisCenter.Services;
using MedlinkDialysisCenter.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MedlinkDialysisCenter.Controllers
{
    public class PhConsumptionController(IPhConsumptionService phService) : Controller
    {
        // GET: /PhConsumption?year=2025
        public async Task<IActionResult> Index(int year = 0)
        {
            if (year == 0) year = DateTime.Now.Year;
            var vm = await phService.GetIndexAsync(year);
            return View(vm);
        }

        // GET: /PhConsumption/Details/5?year=2025
        public async Task<IActionResult> Details(int id, int year = 0)
        {
            if (year == 0) year = DateTime.Now.Year;
            var summary = await phService.GetSummaryAsync(id, year);
            if (summary is null) return NotFound();
            return View(summary);
        }

        // GET: /PhConsumption/AddSession/5
        public IActionResult AddSession(int patientId)
        {
            var vm = new PhConsumptionViewModel
            {
                PatientId = patientId,
                ConsumptionType = PhConsumptionType.OwnCenter,
                YearCovered = DateTime.Now.Year
            };
            return View(vm);
        }

        // POST: /PhConsumption/AddSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSession(PhConsumptionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = User.Identity?.Name ?? "System";
            var (success, error) = await phService.AddSessionAsync(vm, user);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error!);
                return View(vm);
            }

            TempData["Success"] = "Session recorded successfully.";
            return RedirectToAction(nameof(Details), new { id = vm.PatientId });
        }

        // GET: /PhConsumption/AddTransferee/5
        public IActionResult AddTransferee(int patientId)
        {
            var vm = new PhConsumptionViewModel
            {
                PatientId = patientId,
                ConsumptionType = PhConsumptionType.OtherCenter,
                YearCovered = DateTime.Now.Year
            };
            return View(vm);
        }

        // POST: /PhConsumption/AddTransferee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransferee(PhConsumptionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = User.Identity?.Name ?? "System";
            var (success, error) = await phService.AddTransfereeSessionsAsync(vm, user);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error!);
                return View(vm);
            }

            TempData["Success"] = "Transferee sessions recorded successfully.";
            return RedirectToAction(nameof(Details), new { id = vm.PatientId });
        }

        // GET: /PhConsumption/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await phService.GetByIdAsync(id);
            if (vm is null) return NotFound();
            return View(vm);
        }

        // POST: /PhConsumption/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PhConsumptionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var (success, error) = await phService.UpdateAsync(vm);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error!);
                return View(vm);
            }

            TempData["Success"] = "Record updated successfully.";
            return RedirectToAction(nameof(Details), new { id = vm.PatientId });
        }

        // POST: /PhConsumption/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int patientId)
        {
            var (success, error) = await phService.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = success ? "Record deleted." : error;
            return RedirectToAction(nameof(Details), new { id = patientId });
        }
    }
}