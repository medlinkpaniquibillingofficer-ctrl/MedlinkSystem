using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using MedlinkDialysisCenter.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Services
{
    public class VaccineService
    {
        private readonly AppDbContext _db;

        public VaccineService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<PatientVaccineModel>> GetByPatientIdAsync(int patientId) =>
            await _db.PatientVaccines
                .Where(v => v.PatientId == patientId)
                .OrderBy(v => v.DateGiven)
                .ToListAsync();

        public async Task SaveAsync(int patientId, List<VaccineEntryViewModel> vaccines)
        {
            var patientExists = await _db.Patients
                .AnyAsync(p => p.PatientId == patientId && !p.IsDeleted);
            if (!patientExists) throw new InvalidOperationException("Patient not found.");

            var existing = await _db.PatientVaccines
                .Where(v => v.PatientId == patientId)
                .ToListAsync();

            _db.PatientVaccines.RemoveRange(existing);

            var toAdd = vaccines
                .Where(v => !v.IsDeleted
                    && !string.IsNullOrWhiteSpace(v.VaccineName)
                    && v.DateGiven.HasValue)
                .Select(v => new PatientVaccineModel
                {
                    PatientId = patientId,
                    VaccineName = v.VaccineName.Trim(),
                    Dose = v.Dose,
                    DateGiven = v.DateGiven!.Value,
                    CreatedAt = DateTime.Now
                });

            _db.PatientVaccines.AddRange(toAdd);
            await _db.SaveChangesAsync();
        }

        public VaccineViewModel MapToViewModel(List<PatientVaccineModel> vaccines, int patientId, string patientCode, string patientName)
        {
            return new VaccineViewModel
            {
                PatientId = patientId,
                PatientCode = patientCode,
                PatientName = patientName,
                Vaccines = vaccines.Select(v => new VaccineEntryViewModel
                {
                    PatientVaccineId = v.PatientVaccineId,
                    VaccineName = v.VaccineName,
                    Dose = v.Dose,
                    DateGiven = v.DateGiven
                }).ToList()
            };
        }
    }
}