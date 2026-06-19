using MedlinkDialysisCenter.Data;
using MedlinkDialysisCenter.Models;
using MedlinkDialysisCenter.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MedlinkDialysisCenter.Services
{
    public class HepaTestService
    {
        private readonly AppDbContext _db;

        public HepaTestService(AppDbContext db){
            _db = db;
        }

        public async Task<HepaTestModel?> GetByPatientIdAsync(int patientId) =>
            await _db.HepaTests.FirstOrDefaultAsync(h => h.PatientId == patientId);

        public async Task SaveAsync(HepaTestViewModel vm){
            var patientExists = await _db.Patients.AnyAsync(p => p.PatientId == vm.PatientId && !p.IsDeleted);
            if (!patientExists) throw new InvalidOperationException("Patient not found.");

            var existing = await GetByPatientIdAsync(vm.PatientId);

            if (existing == null){
                // First time — create
                var test = new HepaTestModel
                {
                    PatientId       = vm.PatientId,
                    HepaBTested     = vm.HepaBTested,
                    HepaBResult     = vm.HepaBTested ? vm.HepaBResult : null,
                    AntiHBSTested   = vm.AntiHBSTested,
                    AntiHBSResult   = vm.AntiHBSTested ? vm.AntiHBSResult : null,
                    HepaCTested     = vm.HepaCTested,
                    HepaCResult     = vm.HepaCTested ? vm.HepaCResult : null,
                    Remarks         = vm.Remarks,
                    CreatedAt       = DateTime.Now
                };
                _db.HepaTests.Add(test);
            }
            else{
                // Already exists — update
                existing.HepaBTested    = vm.HepaBTested;
                existing.HepaBResult    = vm.HepaBTested ? vm.HepaBResult : null;
                existing.AntiHBSTested  = vm.AntiHBSTested;
                existing.AntiHBSResult  = vm.AntiHBSTested ? vm.AntiHBSResult : null;
                existing.HepaCTested    = vm.HepaCTested;
                existing.HepaCResult    = vm.HepaCTested ? vm.HepaCResult : null;
                existing.Remarks        = vm.Remarks;
                existing.UpdatedAt      = DateTime.Now;
            }
            await _db.SaveChangesAsync();
        }

        // Map entity → ViewModel (for pre-filling the edit form)
        public HepaTestViewModel MapToViewModel(HepaTestModel test, string patientCode, string patientName)
        {
            return new HepaTestViewModel
            {
                PatientId       = test.PatientId,
                PatientCode     = patientCode,
                PatientName     = patientName,
                HepaBTested     = test.HepaBTested,
                HepaBResult     = test.HepaBResult,
                AntiHBSTested   = test.AntiHBSTested,
                AntiHBSResult   = test.AntiHBSResult,
                HepaCTested     = test.HepaCTested,
                HepaCResult     = test.HepaCResult,
                Remarks         = test.Remarks,
            };
        }
    }
}