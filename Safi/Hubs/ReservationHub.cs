using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Safi.Models;
namespace Safi.Hubs;

public class ReservationHub : Hub
{
    private readonly SafiContext _context;

    public ReservationHub(SafiContext context)
    {
        _context = context;
    }

    public async Task TestMe(string someRandomText)
    {
        await Clients.All.SendAsync(
            $"{this.Context.User?.Identity?.Name} : {someRandomText}",
            CancellationToken.None);
    }

    public async Task JoinDoctorDayGroup(string DoctorId, string Day)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{DoctorId}_{Day}");
    }

    public async Task LeaveDoctorDayGroup(string DoctorId, string Day)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{DoctorId}_{Day}");
    }

    public async Task GetAvailableSlots(string doctorId, string day)
    {
        var targetDate = DateOnly.Parse(day);
        var dateTime = targetDate.ToDateTime(TimeOnly.MinValue);

        var reservations = await _context.Reservations
            .Where(r => r.DoctorId == doctorId && r.Time.Date == dateTime.Date)
            .Select(r => new
            {
                r.Id,
                r.Time,
                r.Status,
                r.PatientId,
                IsAvailable = r.Status != "Reserved"
            })
            .ToListAsync();
        await Clients.Caller.SendAsync("ReceiveSlots", reservations);
    }

    public async Task ReserveSlot(int reservationId, string patientId)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null)
        {
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = false,
                Message = "Slot not found",
                ReservationId = reservationId
            });
            return;
        }

        if (reservation.Status == "Reserved")
        {
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = false,
                Message = "This slot has already been reserved by another patient",
                ReservationId = reservationId
            });
            return;
        }

        try
        {
            // Use patientId parameter (not reservation.PatientId which may be null/old value)
            var user = await _context.Patients
                .Include(p => p.Departments)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            // Include Department navigation property
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == reservation.DoctorId);

            if (user == null || doctor == null || doctor.Department == null)
            {
                await Clients.Caller.SendAsync("ReservationResult", new
                {
                    Success = false,
                    Message = "Patient, Doctor, or Department not found",
                    ReservationId = reservationId
                });
                return;
            }

            reservation.PatientId = patientId;
            reservation.Status = "Reserved";

            // Check if department already exists in patient's departments by Id
            var departmentExists = (user.Departments ?? Enumerable.Empty<Department>()).Any(d => d.Id == doctor.Department.Id);
            if (!departmentExists)
            {
                user.Departments ??= new List<Department>();
                user.Departments.Add(doctor.Department);
            }
            await _context.SaveChangesAsync();

            var dayStr = reservation.Time.ToString("yyyy-MM-dd");

            // Notify the caller of success
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = true,
                Message = "Reservation successful!",
                ReservationId = reservationId,
                PatientId = patientId
            });

            // Notify everyone in the group that this slot is now taken
            await Clients.Group($"{reservation.DoctorId}_{dayStr}")
                .SendAsync("SlotReserved", new
                {
                    ReservationId = reservation.Id,
                    Message = "This slot has been reserved by another patient",
                    DoctorId = reservation.DoctorId,
                    Day = dayStr,
                    Time = reservation.Time,
                    PatientId = patientId
                });
        }
        catch (DbUpdateConcurrencyException)
        {
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = false,
                Message = "Race condition: slot was reserved by another patient",
                ReservationId = reservationId
            });
        }
    }
}

