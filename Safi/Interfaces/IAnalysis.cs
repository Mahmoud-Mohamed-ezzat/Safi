using Safi.Dto.Analysis;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IAnalysis
    {
        Task<List<Analysis>> GetAllAsync();
        Task<Analysis?> GetByIdAsync(int id);
        Task<Analysis> CreateAsync(CreateAnalysisDto dto);
        Task<Analysis?> UpdateAsync(int id, UpdateAnalysisDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<Analysis>> GetByPatientIdAsync(string patientId);
        Task<List<Analysis>> GetByPatientCustomIdAsync(int customId);
    }
}
