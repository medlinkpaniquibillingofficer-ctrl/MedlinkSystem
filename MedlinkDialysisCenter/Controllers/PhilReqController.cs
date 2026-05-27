using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Controllers
{
    [Authorize(Roles = "Admin")]
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
                .OrderByDescending(r => r.Patient.PatientId)
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
        public async Task<IActionResult> Create(PHRequirement model){
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

        public IActionResult ExportPhilHealthToExcel(){

            var records = _db.PHRequirements.Include(r => r.Patient).ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("PhilHealth Tracker");

            // --- Headers ---
            var headers = new[]
            {
        "Patient ID", "Patient Name", "Category",
        "CSF", "CF2", "MDR", "PhilHealth ID",
        "Receipt (6mo)", "Cert. Contrib.", "SC ID",
        "CSF w/ Employer", "PDD Reg.", "PH Consumption", "Status"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromArgb(37, 99, 235); // blue-600
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.WrapText = true;
            }

            // Left-align first 3 headers
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(1, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell(1, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            // --- Data rows ---
            string Bool(bool val) => val ? "✓" : "✗";

            for (int i = 0; i < records.Count; i++)
            {
                var r = records[i];
                var row = i + 2;

                ws.Cell(row, 1).Value = r.Patient.PatientId.ToString("D4");
                ws.Cell(row, 2).Value = r.Patient.FullName;
                ws.Cell(row, 3).Value = r.MemberCategory ?? "—";
                ws.Cell(row, 4).Value = Bool(r.HasCSF);
                ws.Cell(row, 5).Value = Bool(r.HasCF2);
                ws.Cell(row, 6).Value = Bool(r.HasMDR);
                ws.Cell(row, 7).Value = Bool(r.HasPhilhealthId);
                ws.Cell(row, 8).Value = Bool(r.HasReceipt6Mos);
                ws.Cell(row, 9).Value = Bool(r.HasCertMonthlyContrib);
                ws.Cell(row, 10).Value = Bool(r.HasSCId);
                ws.Cell(row, 11).Value = Bool(r.HasCSFEmployerSig);
                ws.Cell(row, 12).Value = Bool(r.HasPDDRegistration);
                ws.Cell(row, 13).Value = Bool(r.HasPhilhealthConsumption);
                ws.Cell(row, 14).Value = r.IsComplete ? "COMPLETE" : "INCOMPLETE";

                // Center boolean + status columns
                for (int col = 4; col <= 14; col++)
                    ws.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Color-code the Status cell
                var statusCell = ws.Cell(row, 14);
                statusCell.Style.Font.Bold = true;
                if (r.IsComplete)
                {
                    statusCell.Style.Font.FontColor = XLColor.FromArgb(21, 128, 61);   // green-700
                    statusCell.Style.Fill.BackgroundColor = XLColor.FromArgb(220, 252, 231); // green-100
                }
                else
                {
                    statusCell.Style.Font.FontColor = XLColor.FromArgb(185, 28, 28);   // red-700
                    statusCell.Style.Fill.BackgroundColor = XLColor.FromArgb(254, 226, 226); // red-100
                }

                // Zebra striping (skip status cell so color stands out)
                if (i % 2 == 1)
                {
                    for (int col = 1; col <= 13; col++)
                        ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(248, 250, 252);
                }
            }

            // --- Summary rows ---
            var total = records.Count;
            var complete = records.Count(r => r.IsComplete);
            var incomplete = total - complete;
            var rate = total > 0 ? $"{(int)((double)complete / total * 100)}%" : "0%";

            var sumRow = total + 3;
            var summaryData = new[]
            {
        ("Total Records:",    total.ToString()),
        ("Complete:",         complete.ToString()),
        ("Incomplete:",       incomplete.ToString()),
        ("Completion Rate:",  rate),
    };

            foreach (var (label, value) in summaryData)
            {
                ws.Cell(sumRow, 1).Value = label;
                ws.Cell(sumRow, 1).Style.Font.Bold = true;
                ws.Cell(sumRow, 2).Value = value;
                sumRow++;
            }

            // --- Polish ---
            ws.Columns().AdjustToContents();
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 24); // ensure name column is readable
            ws.SheetView.FreezeRows(1);

            var dataRange = ws.Range(1, 1, records.Count + 1, headers.Length);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var fileName = $"PhilHealth_Tracker_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}
