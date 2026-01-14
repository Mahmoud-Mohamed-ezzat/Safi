using Safi.Dto.Reservation;
using Safi.Models;

namespace Safi.Interfaces
{
    public interface IReservation
    {
        public Task<Reservation> CreateOneReservation(CreateReservationDto dto);
        public Task<List<Reservation>> CreateManyReservations(CreatemanyReservationsDto dto);
        public Task<bool> UpdateManyReservationsByAvailableTimeId(int availableTimeId, UpdateManyReservationsDto dto);
        public Task<bool> DeleteManyReservationsByAvailableTimeId(int availableTimeId);
        public Task<bool> Assignreservationforpatient(AssignReservationForpatient dto);
        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByPatientId(string patientId);
        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByDoctorId(string doctorId);
        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByPatientIdAndDoctorId(string patientId, string doctorId);
        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByPatientIdAndDate(string patientId, DateTime date);
        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByDoctorIdAndDate(string doctorId, DateTime date);
        public Task<List<reservationInfoforgetpatientReservations>> GetReservationByDate(DateTime date);
        public Task<string?> addDepartmenttoPatientDepartmentWhenReservationIsCreated(addDepartmenttoPatientDepartmentWhenReservationIsCreatedDto dto);
    }
}
