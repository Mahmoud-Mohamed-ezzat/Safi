using Safi.Interfaces;
using Safi.Models;
using System.Text.Json;
using Safi.Dto.OutBoxDto;
using Safi.Dto.EmailDto;
using Safi.Dto.Bill;
using Safi.Dto.AppointmentToRoom;
using Microsoft.EntityFrameworkCore;
namespace Safi.Services
{
    public class BillBackgroundService : BackgroundService
    {
        private readonly ILogger<BillBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public BillBackgroundService(ILogger<BillBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatchAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox processor failed");
                }
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task ProcessBatchAsync(CancellationToken stoppingToken)
        {
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SafiContext>();
            var billingService = scope.ServiceProvider.GetRequiredService<IBill>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var priceService = scope.ServiceProvider.GetRequiredService<IPrices>();
            var OutboxItems = await dbContext.OutboxMessages
                .Where(p => p.ProcessedAt == null && p.RetryCount < 3)
                .Take(50)
                .ToListAsync(stoppingToken);
            foreach (var msg in OutboxItems)
            {
                try
                {
                    if (msg.Type == "BillAndEmailForReservation")
                    {
                        var data = JsonSerializer.Deserialize<BillAndEmailForReservationDto>(msg.Payload);
                        var price = await priceService
                               .GetPriceNowAsync(data.DepartmentName + "Reservation");
                        var billId = await billingService
                        .CheckThisBillActive(data.PatientId);

                        var reservationDetails = $"\n Reservation {data.PatientName} Done with {data.DoctorName} on {data.Time} price={price}";

                        if (billId != 0)
                        {
                            await billingService.AddPriceTobill(new UpdateBillDto
                            {
                                Id = billId,
                                TotalAmount = price,
                                PatientId = data.PatientId,
                                Details = reservationDetails,
                            });
                        }
                        else
                        {
                            await billingService.CreateBill(new createbillDto
                            {
                                st_Date = DateTime.UtcNow,
                                Status = "open",
                                Details = reservationDetails,
                                TotalAmount = price,
                                currency = "EGP",
                                PatientId = data.PatientId,
                            });
                        }

                        if (data.PatientEmail != null)
                        {
                            await emailService.SendEmailAsync(new SendEmailDto
                            {
                                ToEmail = data.PatientEmail,
                                Subject = "Reservation Confirmation",
                                Body = $"Dear {data.PatientName},\nYour reservation with Dr. {data.DoctorName} on {data.Time} has been confirmed.\n\nThank you for choosing Safi."
                            });
                        }
                        if (data.DoctorEmail != null)
                        {
                            await emailService.SendEmailAsync(new SendEmailDto
                            {
                                ToEmail = data.DoctorEmail,
                                Subject = "New Reservation Notification",
                                Body = $"Dear Dr. {data.DoctorName},\nYou have a new reservation from patient {data.PatientName} on {data.Time}."
                            });
                        }
                        msg.ProcessedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                    else if (msg.Type == "BillAndEmailForAppointment")
                    {
                        var data = JsonSerializer.Deserialize<BillAndEmailForAppointmentDto>(msg.Payload);
                        if (data == null) continue;

                        var billId = await billingService.CheckThisBillActive(data.PatientId);

                        // Calculate total amount based on hourly rates
                        var calculator = scope.ServiceProvider.GetRequiredService<PriceSegmentCalculator>();
                        var totalAmount = await calculator.Calculate(new AppointmentToRoomDto
                        {
                            StartTime = data.StartTime,
                            EndTime = data.EndTime
                        }, data.RoomType + data.DepartmentName);

                        // Update the appointment record
                        var appointment = await dbContext.AppointmentToRooms.FirstOrDefaultAsync(a => a.Id == data.AppointmentId);
                        if (appointment != null)
                        {
                            appointment.TotalPrice = totalAmount;
                        }

                        var details = $"\n Appointment in {data.RoomType} #{data.RoomNumber} with Dr. {data.DoctorName} in department {data.DepartmentName} started at {data.StartTime:f} ended at {data.EndTime:f} price={totalAmount}";
                        
                        if (billId != 0)
                        {
                            await billingService.AddPriceTobill(new UpdateBillDto
                            {
                                Id = billId,
                                TotalAmount = totalAmount,
                                PatientId = data.PatientId,
                                Details = details
                            });
                        }
                        else
                        {
                            await billingService.CreateBill(new createbillDto
                            {
                                st_Date = DateTime.UtcNow,
                                Status = "open",
                                Details = details,
                                TotalAmount = totalAmount,
                                currency = "EGP",
                                PatientId = data.PatientId,
                            });
                        }

                        if (data.PatientEmail != null)
                        {
                            await emailService.SendEmailAsync(new SendEmailDto
                            {
                                ToEmail = data.PatientEmail,
                                Subject = "Appointment Confirmation",
                                Body = $"Dear {data.PatientName},\nYour appointment in {data.RoomType} #{data.RoomNumber} with Dr. {data.DoctorName} has been completed.\n\nTotal Amount: {totalAmount} EGP\n\nThank you for choosing Safi."
                            });
                        }
                        if (data.DoctorEmail != null)
                        {
                            await emailService.SendEmailAsync(new SendEmailDto
                            {
                                ToEmail = data.DoctorEmail,
                                Subject = "Appointment Completed",
                                Body = $"Dear Dr. {data.DoctorName},\nYour appointment with patient {data.PatientName} has been completed and billed."
                            });
                        }

                        msg.ProcessedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                    else if (msg.Type == "CreateAppointmentEmail")
                    {
                        var data = JsonSerializer.Deserialize<CreateAppointmentEmailDto>(msg.Payload);
                        if (data != null && !string.IsNullOrEmpty(data.DoctorEmail))
                        {
                            await emailService.SendEmailAsync(new SendEmailDto
                            {
                                ToEmail = data.DoctorEmail,
                                Subject = "New Appointment Created",
                                Body = $@"
                                    <h2>New Appointment Notification</h2>
                                    <p>Dear Dr. {data.DoctorName},</p>
                                    <p>A new appointment has been created for you.</p>
                                    <p><strong>Appointment Details:</strong></p>
                                    <ul>
                                        <li><strong>Patient:</strong> {data.PatientName}</li>
                                        <li><strong>Room:</strong> {data.RoomType} #{data.RoomNumber}</li>
                                        <li><strong>Department:</strong> {data.DepartmentName}</li>
                                        <li><strong>Start Time:</strong> {data.StartTime:MMMM dd, yyyy hh:mm tt}</li>
                                    </ul>
                                    <p>Please be prepared for this appointment.</p>
                                    <p>Best regards,<br/>Safi Hospital Team</p>"
                            });
                        }
                        msg.ProcessedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                    else if (msg.Type == "CancelReservationAndBill")
                    {
                        var data = JsonSerializer.Deserialize<CancelReservationAndBillDto>(msg.Payload);
                        if (data == null)
                        {
                            _logger.LogWarning("Outbox message {Id} has empty or invalid payload", msg.Id);
                            msg.ProcessedAt = DateTime.UtcNow;
                            await dbContext.SaveChangesAsync(stoppingToken);
                            continue;
                        }

                        // Fallback: If DepartmentName is "Unknown", try to fetch it from the Doctor record
                        if (string.IsNullOrEmpty(data.DepartmentName) || data.DepartmentName.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
                        {
                            // Try to fetch department by DoctorId first
                            if (!string.IsNullOrEmpty(data.DoctorId))
                            {
                                var doctor = await dbContext.Doctors
                                    .Include(d => d.Department)
                                    .FirstOrDefaultAsync(d => d.Id == data.DoctorId, stoppingToken);

                                if (doctor?.Department != null)
                                {
                                    data.DepartmentName = doctor.Department.Name;
                                    _logger.LogInformation("Recovered department name '{Dept}' for doctor {Id}", data.DepartmentName, data.DoctorId);
                                }
                            }

                            // If still unknown, try to fetch department by DoctorName
                            if (string.IsNullOrEmpty(data.DepartmentName) || data.DepartmentName.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.IsNullOrEmpty(data.DoctorName))
                                {
                                    var doctor = await dbContext.Doctors
                                        .Include(d => d.Department)
                                        .FirstOrDefaultAsync(d => d.Name == data.DoctorName, stoppingToken);

                                    if (doctor?.Department != null)
                                    {
                                        data.DepartmentName = doctor.Department.Name;
                                        data.DoctorId = doctor.Id;
                                        _logger.LogInformation("Recovered department name '{Dept}' for doctor '{DoctorName}'", data.DepartmentName, data.DoctorName);
                                    }
                                }
                            }
                        }

                        var price = await priceService.GetPriceNowAsync(data.DepartmentName + "Reservation");
                        if (price == 0)
                        {
                            _logger.LogWarning("Price not found for service: {Service}. Payload: {Payload}", data.DepartmentName + "Reservation", msg.Payload);
                        }

                        var bill = await dbContext.Bills
                            .FirstOrDefaultAsync(b => b.PatientId == data.PatientId && (b.Status == "open" || b.Status == "Open"), stoppingToken);

                        if (bill != null)
                        {
                            bill.TotalAmount -= price;
                            if (bill.TotalAmount < 0) bill.TotalAmount = 0;

                            // Remove specific detail line
                            if (!string.IsNullOrEmpty(bill.Details))
                            {
                                var formattedTime = data.ReservationTime.ToString("yyyy-MM-dd hh:mm tt");
                                var detailToRemove = $"\n Reservation {data.PatientName} Done with {data.DoctorName} on {formattedTime} price={price}";

                                bill.Details = bill.Details.Replace(detailToRemove, "").Trim();
                            }

                            // If bill is empty or amount is 0 and no details, consider closing/deleting
                            if (bill.TotalAmount <= 0 && (string.IsNullOrEmpty(bill.Details) || bill.Details.Trim().Length == 0))
                            {
                                dbContext.Bills.Remove(bill);
                            }
                        }

                        if (data.PatientEmail != null)
                        {
                            await emailService.SendEmailAsync(new SendEmailDto
                            {
                                ToEmail = data.PatientEmail,
                                Subject = "Reservation Cancelled – Safi Hospital",
                                Body = $"Dear {data.PatientName},\n\nWe regret to inform you that your reservation with Dr. {data.DoctorName} on {data.ReservationTime:f} has been cancelled.\n\nWe apologize for any inconvenience caused.\n\nRegards,\nSafi Hospital"
                            });
                        }

                        msg.ProcessedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {Id}", msg.Id);
                    msg.Error = ex.Message;
                    msg.FailedAt = DateTime.UtcNow;
                    msg.RetryCount++;
                    await dbContext.SaveChangesAsync(stoppingToken);
                }

            }

        }
    }
}
