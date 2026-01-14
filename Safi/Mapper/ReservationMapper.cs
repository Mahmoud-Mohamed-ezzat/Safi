using Safi.Dto.Reservation;
using Safi.Dto.AvailableTimeOFDoctor;
using Safi.Models;

namespace Safi.Mapper;

public static class ReservationMapper
{

    public static Reservation ToCreateReservationDto(this CreateReservationDto dto)
    {
        return new Reservation
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            Time = dto.Time,
            Status = dto.Status
        };
    }
    public static CreatemanyReservationsDto CreateManyReservationsFromDoctorTimesDto(this CreateAvailableTimeDto dto)
    {
        var createmanyreservations = new CreatemanyReservationsDto();
        createmanyreservations.DoctorId = dto.DoctorId;
        createmanyreservations.slots = dto.Slots;
        createmanyreservations.StartTime = dto.StartTime;
        createmanyreservations.EndTime = dto.EndTime;
        return createmanyreservations;
    }

    public static UpdateManyReservationsDto ToUpdateManyReservationsDto2(this UpdateAvailableTimeDto2 dto, string doctorId)
    {
        return new UpdateManyReservationsDto
        {
            DoctorId = doctorId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            slots = dto.Slots
        };
    }

    public static UpdateManyReservationsDto ToUpdateManyReservationsDto(this UpdateAvailableTimeDto dto)
    {
        return new UpdateManyReservationsDto
        {
            DoctorId = dto.DoctorId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            slots = dto.Slots
        };
    }
}