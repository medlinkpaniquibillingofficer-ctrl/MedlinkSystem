using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
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
            var patients = await _db.Patients.OrderByDescending(p => p.PatientCode).ToListAsync();
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

        public IActionResult Celebrants(){
            var currentMonth = DateTime.Now.Month;

            var celebrants = _db.Patients
                .Where(p => p.DateOfBirth.HasValue &&
                            p.DateOfBirth.Value.Month == currentMonth)
                .OrderBy(p => p.DateOfBirth.Value.Day)
                .ToList();

            return View(celebrants);
        }

        public IActionResult ExportToExcel()
        {
            var patients = _db.Patients.ToList(); // or however you fetch patients

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Patients");

            // --- Header row ---
            var headers = new[] { "Patient ID", "Full Name", "Gender", "PhilHealth No.", "Contact No.", "Diagnosis", "Registered" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(37, 99, 235); // blue-600
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }

            // --- Data rows ---
            for (int i = 0; i < patients.Count; i++)
            {
                var p = patients[i];
                var row = i + 2;

                ws.Cell(row, 1).Value = p.PatientId.ToString("D4");
                ws.Cell(row, 2).Value = p.FullName;
                ws.Cell(row, 3).Value = p.Gender ?? "—";
                ws.Cell(row, 4).Value = p.PhilhealthNo ?? "—";
                ws.Cell(row, 5).Value = p.ContactNo ?? "—";
                ws.Cell(row, 6).Value = p.Diagnosis ?? "—";
                ws.Cell(row, 7).Value = p.CreatedAt.ToString("yyyy-MM-dd");

                // Zebra striping
                if (i % 2 == 1)
                {
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 250, 252); // slate-50
                }
            }

            // --- Stat summary (mirrors your stat cards) ---
            var summaryRow = patients.Count + 3;
            ws.Cell(summaryRow, 1).Value = "Total Patients:";
            ws.Cell(summaryRow, 1).Style.Font.Bold = true;
            ws.Cell(summaryRow, 2).Value = patients.Count;

            ws.Cell(summaryRow + 1, 1).Value = "Registered This Month:";
            ws.Cell(summaryRow + 1, 1).Style.Font.Bold = true;
            ws.Cell(summaryRow + 1, 2).Value = patients.Count(p =>
                p.CreatedAt.Month == DateTime.Now.Month && p.CreatedAt.Year == DateTime.Now.Year);

            // --- Auto-fit columns ---
            ws.Columns().AdjustToContents();

            // --- Freeze header row ---
            ws.SheetView.FreezeRows(1);

            // --- Table border ---
            var dataRange = ws.Range(1, 1, patients.Count + 1, headers.Length);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"Patients_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}