using Safi.Dto.Account;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IDoctor
    {
        Task<bool> RateDoctorAsync(string doctorId, int rating);
        Task<Doctor?> GetByIdAsync(string doctorId);
        Task<List<GetPatientsDto>> GetallpatientsdealwithDoctor(string doctorId);
        Task<List<GetPatientsDto>> GetallpatientsdealwithDoctorReservation(string doctorId);
        Task<bool> DeleteDoctorAsync(string doctorId);
    }
}
