using ClosedXML.Excel;
using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;

namespace MedlinkDialysisCenter.Services
{
    public class PatientService
    {
        private readonly AppDbContext _db;

        public PatientService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResultModel<PatientViewModel>> GetActivePatients(
    int page, int pageSize, string? search = null)
        {
            var query = _db.Patients
                .AsNoTracking()
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.FirstName.Contains(search) ||
                    p.LastName.Contains(search) ||
                    (p.PhilhealthNo != null && p.PhilhealthNo.Contains(search)) ||
                    (p.Diagnosis != null && p.Diagnosis.Contains(search)));

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.PatientCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PatientViewModel
                {
                    PatientId = p.PatientId,
                    FullName = p.FullName,
                    PatientCode = p.PatientCode,
                    FirstInitial = p.FirstName.Substring(0, 1),
                    LastInitial = p.LastName.Substring(0, 1),
                    PhilhealthNo = p.PhilhealthNo,
                    ContactNo = p.ContactNo,
                    Diagnosis = p.Diagnosis,
                    Nephrologist = p.Nephrologist,
                    HasVaccines = p.Vaccines.Any(),
                    HepaTest = p.HepaTest,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new PagedResultModel<PatientViewModel>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                Search = search
            };
        }

        public async Task<Patient?> GetByCode(string patientCode) =>
            await _db.Patients
            .Include(p => p.HepaTest)
            .Include(p => p.Vaccines)
            .FirstOrDefaultAsync(p => p.PatientCode == patientCode);

        public async Task<Patient?> GetById(int id) =>
            await _db.Patients.FindAsync(id);

        public async Task CreatePatient(Patient patient)
        {
            patient.PhilhealthNo = patient.PhilhealthNo?.Replace(" ", "");
            patient.ContactNo = patient.ContactNo?.Replace(" ", "");
            patient.CreatedAt = DateTime.Now;

            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();
        }

        public async Task UpdatePatient(Patient patient)
        {
            patient.PhilhealthNo = patient.PhilhealthNo?.Replace(" ", "");
            patient.ContactNo = patient.ContactNo?.Replace(" ", "");

            _db.Patients.Update(patient);
            await _db.SaveChangesAsync();
        }

        public async Task SoftDelete(int id)
        {
            var patient = await GetById(id);
            if (patient == null) return;

            patient.IsDeleted = true;
            patient.DeletedAt = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        public async Task Restore(int id){
            var patient = await GetById(id);
            if (patient == null) return;

            patient.IsDeleted = false;
            patient.DeletedAt = null;
            await _db.SaveChangesAsync();
        }

        public async Task<List<Patient>> GetDeletedPatients() =>
            await _db.Patients
                .Where(p => p.IsDeleted)
                .OrderByDescending(p => p.DeletedAt)
                .ToListAsync();

        public List<Patient> GetCelebrantsThisMonth(){
            var currentMonth = DateTime.Now.Month;
            return _db.Patients
                .Where(p => !p.IsDeleted && p.DateOfBirth.HasValue &&
                            p.DateOfBirth.Value.Month == currentMonth)
                .OrderBy(p => p.DateOfBirth!.Value.Day)
                .ToList();
        }

        public byte[] ExportToExcel(){
            var patients = _db.Patients.Where(p => !p.IsDeleted).ToList();

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
            return stream.ToArray();
        }
    }
}