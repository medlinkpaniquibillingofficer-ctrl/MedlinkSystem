using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Enums;
using MedlinkDialysisCenter.Models;
using MedlinkDialysisCenter.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Services
{
    public class PhConsumptionService(AppDbContext db) : IPhConsumptionService
    {
        private const int MaxSessions = 156;

        // ── Summary ─────────────────────────────────────────────────────────

        public async Task<PhSummaryViewModel?> GetSummaryAsync(int patientId, int year)
        {
            var patient = await db.Patients.FindAsync(patientId);
            if (patient is null) return null;

            var records = await db.PhConsumptions
                .Where(x => x.PatientId == patientId && x.YearCovered == year)
                .ToListAsync();

            return BuildSummary(patient, records, year);
        }

        public async Task<PhConsumptionIndexViewModel> GetIndexAsync(int year){
            var records = await db.PhConsumptions
                .Include(x => x.Patient)
                .Where(x => !x.Patient.IsDeleted)
                .Where(x => x.YearCovered == year)
                .ToListAsync();

            var patients = await db.Patients
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            var summaries = patients
                .Select(p => BuildSummary(
                    p,
                    records.Where(r => r.PatientId == p.PatientId).ToList(),
                    year))
                .OrderBy(s => s.PatientCode)
                .ToList();

            return new PhConsumptionIndexViewModel
            {
                YearFilter = year,
                PatientSummaries = summaries
            };
        }

        // ── History ──────────────────────────────────────────────────────────

        public async Task<List<PhConsumptionListViewModel>> GetHistoryAsync(int patientId, int year)
        {
            return await db.PhConsumptions
                .Include(x => x.Patient)
                .Where(x => x.PatientId == patientId && x.YearCovered == year)
                .OrderByDescending(x => x.SessionDate)
                .Select(x => MapToListVm(x))
                .ToListAsync();
        }

        // ── Create ───────────────────────────────────────────────────────────

        public async Task<(bool success, string? error)> AddSessionAsync(PhConsumptionViewModel vm, string createdBy)
        {
            var check = await ValidateRemainingAsync(vm.PatientId, vm.YearCovered, vm.SessionsConsumed);
            if (!check.valid) return (false, check.error);

            vm.ConsumptionType = PhConsumptionType.OwnCenter;
            return await SaveAsync(vm, createdBy);
        }

        public async Task<(bool success, string? error)> AddTransfereeSessionsAsync(PhConsumptionViewModel vm, string createdBy)
        {
            if (string.IsNullOrWhiteSpace(vm.CenterName))
                return (false, "Center name is required for transferee records.");

            var check = await ValidateRemainingAsync(vm.PatientId, vm.YearCovered, vm.SessionsConsumed);
            if (!check.valid) return (false, check.error);

            vm.ConsumptionType = PhConsumptionType.OtherCenter;
            return await SaveAsync(vm, createdBy);
        }

        // ── Edit / Delete ────────────────────────────────────────────────────

        public async Task<PhConsumptionViewModel?> GetByIdAsync(int id)
        {
            var entity = await db.PhConsumptions.Include(x => x.Patient).FirstOrDefaultAsync(x => x.Id == id);
            if (entity is null) return null;

            return new PhConsumptionViewModel
            {
                Id = entity.Id,
                PatientId = entity.PatientId,
                PatientName = entity.Patient?.FullName,
                SessionDate = entity.SessionDate,
                SessionsConsumed = entity.SessionsConsumed,
                ConsumptionType = entity.ConsumptionType,
                CenterName = entity.CenterName,
                Remarks = entity.Remarks,
                YearCovered = entity.YearCovered
            };
        }

        public async Task<(bool success, string? error)> UpdateAsync(PhConsumptionViewModel vm)
        {
            var entity = await db.PhConsumptions.FindAsync(vm.Id);
            if (entity is null) return (false, "Record not found.");

            // Revalidate excluding current record
            var totalUsed = await db.PhConsumptions
                .Where(x => x.PatientId == vm.PatientId && x.YearCovered == vm.YearCovered && x.Id != vm.Id)
                .SumAsync(x => x.SessionsConsumed);

            if (totalUsed + vm.SessionsConsumed > MaxSessions)
                return (false, $"Cannot update. Total sessions would exceed {MaxSessions}.");

            entity.SessionDate = vm.SessionDate;
            entity.SessionsConsumed = vm.SessionsConsumed;
            entity.ConsumptionType = vm.ConsumptionType;
            entity.CenterName = vm.CenterName;
            entity.Remarks = vm.Remarks;
            entity.YearCovered = vm.YearCovered;

            await db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool success, string? error)> DeleteAsync(int id)
        {
            var entity = await db.PhConsumptions.FindAsync(id);
            if (entity is null) return (false, "Record not found.");

            db.PhConsumptions.Remove(entity);
            await db.SaveChangesAsync();
            return (true, null);
        }

        // ── Private Helpers ──────────────────────────────────────────────────

        private async Task<(bool valid, string? error)> ValidateRemainingAsync(int patientId, int year, int sessionsToAdd)
        {
            var totalUsed = await db.PhConsumptions
                .Where(x => x.PatientId == patientId && x.YearCovered == year)
                .SumAsync(x => (int?)x.SessionsConsumed) ?? 0;

            if (totalUsed >= MaxSessions)
                return (false, $"Patient has already exhausted all {MaxSessions} PhilHealth sessions for {year}.");

            if (totalUsed + sessionsToAdd > MaxSessions)
                return (false, $"Only {MaxSessions - totalUsed} session(s) remaining for {year}. Cannot add {sessionsToAdd}.");

            return (true, null);
        }

        private async Task<(bool success, string? error)> SaveAsync(PhConsumptionViewModel vm, string createdBy)
        {
            try
            {
                var entity = new PhConsumptionModel
                {
                    PatientId = vm.PatientId,
                    YearCovered = vm.YearCovered,
                    SessionDate = vm.SessionDate,
                    SessionsConsumed = vm.SessionsConsumed,
                    ConsumptionType = vm.ConsumptionType,
                    CenterName = vm.CenterName,
                    Remarks = vm.Remarks,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now
                };

                db.PhConsumptions.Add(entity);
                await db.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private static PhSummaryViewModel BuildSummary(Patient patient, List<PhConsumptionModel> records, int year)
        {
            return new PhSummaryViewModel
            {
                PatientId = patient.PatientId,
                PatientName = patient.FullName,
                YearCovered = year,
                SessionsFromOtherCenter = records
                    .Where(r => r.ConsumptionType == PhConsumptionType.OtherCenter)
                    .Sum(r => r.SessionsConsumed),
                SessionsFromOwnCenter = records
                    .Where(r => r.ConsumptionType == PhConsumptionType.OwnCenter)
                    .Sum(r => r.SessionsConsumed),
                History = records
                    .OrderByDescending(r => r.SessionDate)
                    .Select(r => MapToListVm(r))
                    .ToList()
            };
        }

        private static PhConsumptionListViewModel MapToListVm(PhConsumptionModel x) => new()
        {
            Id = x.Id,
            PatientId = x.PatientId,
            PatientName = x.Patient?.FullName ?? string.Empty,
            SessionDate = x.SessionDate,
            SessionsConsumed = x.SessionsConsumed,
            ConsumptionType = x.ConsumptionType,
            CenterName = x.CenterName,
            Remarks = x.Remarks,
            YearCovered = x.YearCovered,
            CreatedBy = x.CreatedBy,
            CreatedAt = x.CreatedAt
        };
    }
}