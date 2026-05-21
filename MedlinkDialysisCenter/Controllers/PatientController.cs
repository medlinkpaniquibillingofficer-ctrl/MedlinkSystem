using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Controllers
{
    [Authorize(Roles = "Admin,Nurse")]
    public class PatientsController : Controller
    {
        private readonly AppDbContext _db;

        public PatientsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _db.Patients.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return View(patients);
        }

        // GET: /Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // GET: /Patients/Create
        public IActionResult Create() => View();

        // POST: /Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.PhilhealthNo = patient.PhilhealthNo?.Replace(" ", "");
                patient.ContactNo = patient.ContactNo?.Replace(" ", "");

                patient.CreatedAt = DateTime.Now;
                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: /Patients/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // POST: /Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.PatientId) return NotFound();

            if (ModelState.IsValid)
            {
                patient.PhilhealthNo = patient.PhilhealthNo?.Replace(" ", "");
                patient.ContactNo = patient.ContactNo?.Replace(" ", "");

                _db.Patients.Update(patient);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: /Patients/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null) return NotFound();
            return View(patient);
        }

        // POST: /Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient != null)
            {
                _db.Patients.Remove(patient);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}