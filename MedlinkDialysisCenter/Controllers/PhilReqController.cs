using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;

namespace MedlinkDialysisCenter.Controllers
{
    public class PhilhealthRequirementsController : Controller
    {
        private readonly AppDbContext _db;

        public PhilhealthRequirementsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /PhilhealthRequirements
        public async Task<IActionResult> Index()
        {
            var records = await _db.PHRequirements
                .Include(r => r.Patient)
                .OrderBy(r => r.Patient.LastName)
                .ToListAsync();
            return View(records);
        }

        // GET: /PhilhealthRequirements/Create
        public IActionResult Create()
        {
            ViewBag.Patients = _db.Patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = string.IsNullOrWhiteSpace(p.MiddleName)
                        ? $"{p.LastName}, {p.FirstName}"
                        : $"{p.LastName}, {p.FirstName} {p.MiddleName}"
                })
                .ToList();

            return View("CreateEdit", new PHRequirement());
        }

        // POST: /PhilhealthRequirements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PHRequirement model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    _db.PHRequirements.Add(model);
                    await _db.SaveChangesAsync();
                    TempData["Success"] = "PhilHealth record saved successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["Error"] = "An error occurred while saving the record. Please try again.";
                }
            }
            ViewBag.Patients = _db.Patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = string.IsNullOrWhiteSpace(p.MiddleName)
                        ? $"{p.LastName}, {p.FirstName}"
                        : $"{p.LastName}, {p.FirstName} {p.MiddleName}"
                })
                .ToList();
            return View("CreateEdit", model);
        }

        // GET: /PhilhealthRequirements/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _db.PHRequirements.FindAsync(id);
            if (record == null) return NotFound();
            ViewBag.Patients = new SelectList(_db.Patients.OrderBy(p => p.LastName), "PatientId", "FullName", record.PatientId);
            return View("CreateEdit", record);
        }

        // POST: /PhilhealthRequirements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PHRequirement model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    model.UpdatedAt = DateTime.Now;
                    _db.PHRequirements.Update(model);
                    await _db.SaveChangesAsync();
                    TempData["Success"] = "PhilHealth record updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["Error"] = "An error occurred while updating the record. Please try again.";
                }
            }
            ViewBag.Patients = new SelectList(_db.Patients.OrderBy(p => p.LastName), "PatientId", "FullName", model.PatientId);
            return View("CreateEdit", model);
        }

        // GET: /PhilhealthRequirements/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _db.PHRequirements
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (record == null) return NotFound();
            return View(record);
        }

        // POST: /PhilhealthRequirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _db.PHRequirements.FindAsync(id);
            if (record != null)
            {
                _db.PHRequirements.Remove(record);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
