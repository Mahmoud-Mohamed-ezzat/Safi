using Safi.Models;

namespace Safi.Interfaces
{
    public interface IDoctor
    {
        Task<bool> RateDoctorAsync(string doctorId, int rating);
        Task<Doctor?> GetByIdAsync(string doctorId);
    }
}
