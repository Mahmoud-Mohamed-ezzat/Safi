using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.EmailDto;
using Safi.Interfaces;
using Safi.Models;
namespace Safi.Hubs;

public class ReservationHub : Hub
{
    private readonly SafiContext _context;
    private readonly IEmailService _emailService;

    public ReservationHub(SafiContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
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
                IsAvailable = r.Status != "Reserved" && r.Status != "reserved"
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

        if (reservation.Status != null &&
            reservation.Status.Equals("Reserved", StringComparison.OrdinalIgnoreCase))
        {
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = false,
                Message = "This slot has already been reserved by another patient",
                ReservationId = reservationId
            });
            return;
        }

        // Force user to reserve just one slot max
        var existingReservation = await _context.Reservations
            .AnyAsync(r => r.PatientId == patientId &&
                r.Status != null && r.Status.ToLower() == "reserved" && r.Time.Date == reservation.Time.Date);

        if (existingReservation)
        {
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = false,
                Message = "You already have an active reservation on this day. You cannot reserve more than one slot.",
                ReservationId = reservationId
            });
            return;
        }

        try
        {
            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == reservation.DoctorId);

            if (doctor == null || doctor.Department == null)
            {
                await Clients.Caller.SendAsync("ReservationResult", new
                {
                    Success = false,
                    Message = "Doctor or Department not found",
                    ReservationId = reservationId
                });
                return;
            }

            // Save the reservation — PatientId is a nullable string FK, no Patient row required
            reservation.PatientId = patientId;
            reservation.Status = "Reserved";
            await _context.SaveChangesAsync();

            var dayStr = reservation.Time.ToString("yyyy-MM-dd");

            // Notify caller of success
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = true,
                Message = "Reservation successful!",
                ReservationId = reservationId,
                PatientId = patientId
            });

            // Notify all group members that slot is taken
            await Clients.Group($"{reservation.DoctorId}_{dayStr}")
                .SendAsync("SlotReserved", new
                {
                    ReservationId = reservation.Id,
                    Message = "This slot has been reserved by another patient",
                    DoctorId = reservation.DoctorId,
                    Day = dayStr,
                    Time = reservation.Time,
                    PatientId = patientId,
                });

            // Queue billing/email outbox only if a real patient row exists
            var user = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (user != null)
            {
                var payload = System.Text.Json.JsonSerializer.Serialize(new
                {
                    ReservationId = reservation.Id,
                    PatientId = patientId,
                    DoctorId = reservation.DoctorId,
                    DepartmentId = doctor.DepartmentId,
                    DepartmentName = doctor.Department.Name,
                    Time = reservation.Time.ToString("yyyy-MM-dd hh:mm tt"),
                    DoctorName = doctor.Name,
                    PatientName = user.Name,
                    DoctorEmail = doctor.Email,
                    PatientEmail = user.Email,
                });
                await _context.OutboxMessages.AddAsync(new OutboxMessage
                {
                    Type = "BillAndEmailForReservation",
                    Payload = payload,
                });
                await _context.SaveChangesAsync();
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = false,
                Message = "Race condition: slot was just reserved by another patient. Please refresh.",
                ReservationId = reservationId
            });
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReservationResult", new
            {
                Success = false,
                Message = $"Server error: {ex.Message}",
                ReservationId = reservationId
            });
        }
    }
}
