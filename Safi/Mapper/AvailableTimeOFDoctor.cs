using System;
using Safi.Dto.AvailableTimeOFDoctor;
using Safi.Models;

namespace Safi.Mapper;

public static class AvailableTimeOFDoctor
{
    public  static TimeAvailableOfDoctor AvailableTimeOFDoctorDto(this CreateAvailableTimeDto model)
    {

        return new TimeAvailableOfDoctor
        {
            DoctorId = model.DoctorId ?? throw new ArgumentNullException(nameof(model.DoctorId)),
            Day = model.Day,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Slots = model.Slots


        };
    }
    public static AvailableTimeInfoDto ToAvailableTimeInfoDto(this TimeAvailableOfDoctor model)
    {
        return new AvailableTimeInfoDto
        {
            Id = model.Id,
            DoctorId = model.DoctorId,
            DoctorName = model.Doctor.Name??throw new ArgumentNullException(nameof(model.Doctor.Name)),
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Day = model.Day,
            Slots = model.Slots
        };
    }
     public static SecondaryAvailableTimeInfoDto TimeAvailableOfDoctor2(this TimeAvailableOfDoctor model)
    {
        return new SecondaryAvailableTimeInfoDto
        {
            Id = model.Id,
            DoctorName = model.Doctor.Name??throw new ArgumentNullException(nameof(model.Doctor.Name)),
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Day = model.Day,
            Slots = model.Slots
        };
    }
    public static UpdateAvailableTimeDto UpdateAvailableTimeDto(this TimeAvailableOfDoctor model)
    {
        return new UpdateAvailableTimeDto
        {
            DoctorId = model.DoctorId,
            DoctorName = model.Doctor.Name??throw new ArgumentNullException(nameof(model.Doctor.Name)),
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Day = model.Day,
            Slots = model.Slots
        };
    }
}

