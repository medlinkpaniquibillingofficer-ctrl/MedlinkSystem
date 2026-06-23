using MedlinkDialysisCenter.ViewModels;

namespace MedlinkDialysisCenter.Services
{
    public interface IPhConsumptionService
    {
        // Summary
        Task<PhSummaryViewModel?> GetSummaryAsync(int patientId, int year);
        Task<PhConsumptionIndexViewModel> GetIndexAsync(int year);

        // History
        Task<List<PhConsumptionListViewModel>> GetHistoryAsync(int patientId, int year);

        // Create
        Task<(bool success, string? error)> AddSessionAsync(PhConsumptionViewModel vm, string createdBy);
        Task<(bool success, string? error)> AddTransfereeSessionsAsync(PhConsumptionViewModel vm, string createdBy);

        // Edit / Delete
        Task<PhConsumptionViewModel?> GetByIdAsync(int id);
        Task<(bool success, string? error)> UpdateAsync(PhConsumptionViewModel vm);
        Task<(bool success, string? error)> DeleteAsync(int id);
    }
}