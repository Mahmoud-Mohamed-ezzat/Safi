using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.AppointmentToRoom;
using Safi.Dto.EmailDto;
using Safi.Helpers;
using Safi.Interfaces;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Mapper;
using Safi.Models;
using Safi.Services;
using Safi.Dto.Bill;
using Safi.Dto.OutBoxDto;
using System.Text.Json;

namespace Safi.Hubs
{
    public class AppointmentHub : Hub
    {
        private readonly SafiContext _context;
        private readonly IAppointmentToRoom _appointmentRepo;
        private readonly IAssignWorks _assignRoomRepo;
        private readonly IEmailService _emailService;
        private readonly PriceSegmentCalculator _calculator;
        private readonly IBill _bill;

        public AppointmentHub(SafiContext context, IAppointmentToRoom appointmentRepo, IAssignWorks assignRoomRepo, IEmailService emailService, PriceSegmentCalculator calculator, IBill bill)
        {
            _context = context;
            _appointmentRepo = appointmentRepo;
            _assignRoomRepo = assignRoomRepo;
            _emailService = emailService;
            _calculator = calculator;
            _bill = bill;
        }
        // Join a department-specific room group (e.g., "Rooms_1", "ICUs_1", "Emergencies_1")
        public async Task JoinDepartmentRoomGroup(int departmentId, string roomType)
        {
            var groupName = $"{roomType}s_{departmentId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("JoinedGroup", new { GroupName = groupName, Message = $"Joined {groupName} successfully" });

            // Automatically load available rooms after joining
            await GetAvailableRoomsByDepartment(departmentId, roomType);
        }
        /// Leave a department-specific room group
        public async Task LeaveDepartmentRoomGroup(int departmentId, string roomType)
        {
            var groupName = $"{roomType}s_{departmentId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("LeftGroup", new { GroupName = groupName, Message = $"Left {groupName} successfully" });
        }

        //Get available rooms/ICUs/emergencies by department
        // roomType: "Room", "ICU", "Emergency"
        public async Task GetAvailableRoomsByDepartment(int departmentId, string roomType)
        {
            try
            {
                List<AvailableRoomInfoDto> availableRooms = new List<AvailableRoomInfoDto>();

                if (roomType == "Room")
                {
                    var rooms = await _context.Rooms
                        .Include(r => r.Department)
                        .Include(r => r.AssignWorks!)
                            .ThenInclude(a => a.user)
                        .Where(r => r.GetType() == typeof(Room) &&
                                    r.DepartmentId == departmentId &&
                                    r.Status == RoomStatus.Available)
                        .AsNoTracking()
                        .ToListAsync();

                    availableRooms = rooms.Select(r => r.ToAvailableRoomInfoDto(r.AssignWorks?.ToList() ?? new List<AssignWorks>())).ToList();
                }
                else if (roomType == "ICU")
                {
                    var icus = await _context.Icus
                        .Include(r => r.Department)
                        .Include(r => r.AssignWorks!)
                            .ThenInclude(a => a.user)
                        .Where(r => r.DepartmentId == departmentId && r.Status == RoomStatus.Available)
                        .AsNoTracking()
                        .ToListAsync();

                    availableRooms = icus.Select(r => r.ToAvailableRoomInfoDto(r.AssignWorks?.ToList() ?? new List<AssignWorks>())).ToList();
                }
                else if (roomType == "Emergency")
                {
                    var emergencies = await _context.Emergencies
                        .Include(r => r.Department)
                        .Include(r => r.AssignWorks!)
                            .ThenInclude(a => a.user)
                        .Where(r => r.DepartmentId == departmentId && r.Status == RoomStatus.Available)
                        .AsNoTracking()
                        .ToListAsync();

                    availableRooms = emergencies.Select(r => r.ToAvailableRoomInfoDto(r.AssignWorks?.ToList() ?? new List<AssignWorks>())).ToList();
                }

                await Clients.Caller.SendAsync("ReceiveAvailableRooms", new
                {
                    Success = true,
                    DepartmentId = departmentId,
                    RoomType = roomType,
                    Rooms = availableRooms
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveAvailableRooms", new
                {
                    Success = false,
                    Message = $"Error retrieving available rooms: {ex.Message}"
                });
            }
        }
       //Get reserved room of doctor to release it 
        public async Task GetReservedRoomsByDepartment(int departmentId, string roomType,string Doctorid)
        {
            try
            {
                List<AvailableRoomInfoDto> ReservedRooms = new List<AvailableRoomInfoDto>();

                if (roomType == "Room")
                {
                    var rooms = await _context.Rooms
                        .Include(r => r.Department)
                        .Include(r => r.AssignWorks!)
                            .ThenInclude(a => a.user)
                        .Where(r => r.GetType() == typeof(Room) &&
                                     r.DepartmentId == departmentId &&
                                     r.Status == RoomStatus.Busy&&
                                     r.AssignWorks.Any(a => a.userId == Doctorid))
                        .AsNoTracking()
                        .ToListAsync();

                    ReservedRooms = rooms.Select(r => r.ToAvailableRoomInfoDto(r.AssignWorks?.ToList() ?? new List<AssignWorks>())).ToList();
                }
                else if (roomType == "ICU")
                {
                    var icus = await _context.Icus
                        .Include(r => r.Department)
                        .Include(r => r.AssignWorks!)
                            .ThenInclude(a => a.user)
                        .Where(r => r.DepartmentId == departmentId && r.Status == RoomStatus.Busy &&
                                    r.AssignWorks.Any(a => a.userId == Doctorid))
                        .AsNoTracking()
                        .ToListAsync();

                    ReservedRooms = icus.Select(r => r.ToAvailableRoomInfoDto(r.AssignWorks?.ToList() ?? new List<AssignWorks>())).ToList();
                }
                else if (roomType == "Emergency")
                {
                    var emergencies = await _context.Emergencies
                        .Include(r => r.Department)
                        .Include(r => r.AssignWorks!)
                            .ThenInclude(a => a.user)
                        .Where(r => r.DepartmentId == departmentId && r.Status == RoomStatus.Busy &&
                                    r.AssignWorks.Any(a => a.userId == Doctorid))
                        .AsNoTracking()
                        .ToListAsync();

                    ReservedRooms = emergencies.Select(r => r.ToAvailableRoomInfoDto(r.AssignWorks?.ToList() ?? new List<AssignWorks>())).ToList();
                }

                await Clients.Caller.SendAsync("ReceiveReservedRooms", new
                {
                    Success = true,
                    DepartmentId = departmentId,
                    RoomType = roomType,
                    Rooms = ReservedRooms
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveReservedRooms", new
                {
                    Success = false,
                    Message = $"Error retrieving available rooms: {ex.Message}"
                });
            }
        }
        // Block a room slot (change status to Busy) and return assigned doctors
        public async Task BlockRoomSlot(int roomId, string roomType)
        {
            try
            {
                Room? room = null;

                if (roomType == "Room")
                {
                    room = await _context.Rooms
                        .Include(r => r.Department)
                        .Where(r => r.GetType() == typeof(Room))
                        .FirstOrDefaultAsync(r => r.Id == roomId);
                }
                else if (roomType == "ICU")
                {
                    room = await _context.Icus
                        .Include(r => r.Department)
                        .FirstOrDefaultAsync(r => r.Id == roomId);
                }
                else if (roomType == "Emergency")
                {
                    room = await _context.Emergencies
                        .Include(r => r.Department)
                        .FirstOrDefaultAsync(r => r.Id == roomId);
                }

                if (room == null)
                {
                    await Clients.Caller.SendAsync("BlockRoomResult", new
                    {
                        Success = false,
                        Message = "Room not found"
                    });
                    return;
                }

                if (room.Status == RoomStatus.Busy)
                {
                    await Clients.Caller.SendAsync("BlockRoomResult", new
                    {
                        Success = false,
                        Message = "Room is already busy"
                    });
                    return;
                }

                // Change status to Busy
                room.Status = RoomStatus.Busy;
                await _context.SaveChangesAsync();

                // Get assigned doctors
                var assignedDoctors = await _assignRoomRepo.GetByRoomIdAsync(roomId);

                var groupName = $"{roomType}s_{room.DepartmentId}";

                // Notify caller of success with assigned doctors
                await Clients.Caller.SendAsync("BlockRoomResult", new
                {
                    Success = true,
                    Message = "Room blocked successfully",
                    RoomId = roomId,
                    RoomNumber = room.Number,
                    AssignedDoctors = assignedDoctors.Select(a => new
                    {
                        a.DoctorId,
                        a.DoctorName,
                        a.RoomNumber,
                        a.RoomId
                    }).ToList()
                });

                // Broadcast to all clients in the group that this room is now busy
                await Clients.Group(groupName).SendAsync("RoomStatusChanged", new
                {
                    RoomId = roomId,
                    RoomNumber = room.Number,
                    Status = RoomStatus.Busy,
                    RoomType = roomType,
                    DepartmentId = room.DepartmentId,
                    Message = "Room has been blocked"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                await Clients.Caller.SendAsync("BlockRoomResult", new
                {
                    Success = false,
                    Message = "Concurrency error: Room was already blocked by another user"
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("BlockRoomResult", new
                {
                    Success = false,
                    Message = $"Error blocking room: {ex.Message}"
                });
            }
        }

        // Create appointment with primary doctor after blocking room
        public async Task CreateAppointmentWithDoctor(CreateAppointmentToRoomDto dto, string roomType)
        {
            try
            {
                // Validate room exists and is busy
                Room? room = null;

                if (roomType == "Room")
                {
                    room = await _context.Rooms
                        .Include(r => r.Department)
                        .Where(r => r.GetType() == typeof(Room))
                        .FirstOrDefaultAsync(r => r.Id == dto.RoomId);
                }
                else if (roomType == "ICU")
                {
                    room = await _context.Icus
                        .Include(r => r.Department)
                        .FirstOrDefaultAsync(r => r.Id == dto.RoomId);
                }
                else if (roomType == "Emergency")
                {
                    room = await _context.Emergencies
                        .Include(r => r.Department)
                        .FirstOrDefaultAsync(r => r.Id == dto.RoomId);
                }

                if (room == null)
                {
                    await Clients.Caller.SendAsync("AppointmentCreationResult", new
                    {
                        Success = false,
                        Message = "Room not found"
                    });
                    return;
                }

                if (room.Status != RoomStatus.Busy)
                {
                    await Clients.Caller.SendAsync("AppointmentCreationResult", new
                    {
                        Success = false,
                        Message = "Room must be blocked before creating appointment"
                    });
                    return;
                }

                // Validate doctor is assigned to this room
                var isAssigned = await _assignRoomRepo.IsRoomAssignedToSameDoctorAsync(dto.RoomId, dto.PrimaryDoctorId);
                if (!isAssigned)
                {
                    await Clients.Caller.SendAsync("AppointmentCreationResult", new
                    {
                        Success = false,
                        Message = "Selected doctor is not assigned to this room"
                    });
                    return;
                }

                // Create appointment
                var appointment = await _appointmentRepo.CreateAsync(dto);

                // Load doctor and patient information for email
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == dto.PrimaryDoctorId);
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == dto.PatientId);

                // Broadcast to group
                var groupName = $"{roomType}s_{room.DepartmentId}";
                await Clients.Group(groupName).SendAsync("AppointmentCreated", new
                {
                    RoomId = dto.RoomId,
                    RoomNumber = room.Number,
                    RoomType = roomType,
                    DepartmentId = room.DepartmentId,
                    PatientId = dto.PatientId,
                    PrimaryDoctorId = dto.PrimaryDoctorId,
                    Message = "New appointment created"
                });

                // Find or create bill
                var billId = await _bill.CheckThisBillActive(dto.PatientId);
                if (billId == 0)
                {
                    var newBill = await _bill.CreateBill(
                        new createbillDto
                        {
                            PatientId = dto.PatientId,
                            Status = "Open",
                            Details = $"Bill for appointment in {roomType} #{room.Number} - {room.Department?.Name ?? "N/A"}",
                            currency = "EGP",
                            TotalAmount = 0,
                        });
                    billId = newBill.id;
                }
                else
                {
                    var existingBill = await _context.Bills.FirstOrDefaultAsync(b => b.Id == billId);
                    if (existingBill != null)
                    {
                        existingBill.Details += $"\nAdditional charge for appointment in {roomType} #{room.Number} - {room.Department?.Name ?? "N/A"}";
                    }
                }

                // Link bill to appointment
                var appointmentEntity = await _context.AppointmentToRooms.FirstOrDefaultAsync(a => a.Id == appointment.Id);
                if (appointmentEntity != null)
                {
                    appointmentEntity.BillId = billId;
                    appointment.BillId = billId;
                    await _context.SaveChangesAsync();
                }

                // Add outbox message for doctor email
                var outboxMsg = new OutboxMessage
                {
                    Type = "CreateAppointmentEmail",
                    Payload = System.Text.Json.JsonSerializer.Serialize(new Safi.Dto.OutBoxDto.CreateAppointmentEmailDto
                    {
                        AppointmentId = appointment.Id,
                        PatientId = appointment.PatientId,
                        PatientName = patient?.Name ?? "Unknown Patient",
                        DoctorId = appointment.DoctorId,
                        DoctorName = doctor?.Name ?? "Unknown",
                        DoctorEmail = doctor?.Email,
                        RoomType = roomType,
                        RoomNumber = room.Number.ToString(),
                        DepartmentName = room.Department?.Name ?? "N/A",
                        StartTime = appointment.StartTime ?? EgyptTime.Now
                    })
                };
                _context.OutboxMessages.Add(outboxMsg);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("AppointmentCreationResult", new
                {
                    Success = false,
                    Message = $"Error creating appointment: {ex.Message}"
                });
            }
        }

        // Release a room (change status back to Available, close active appointment if exists)
        public async Task ReleaseRoom(string DoctorId,int roomId, string roomType, CreateReportWhenPatientGetOutRoomDto dto)
        {
            var db = await _context.Database.BeginTransactionAsync();
            try
            {

                // Check for active appointment and close it
                var activeAppointment = await _appointmentRepo.GetActiveAppointmentByRoomIdAsync(roomId,DoctorId);

                if (activeAppointment != null)
                {
                    if (dto.CreatedBy != activeAppointment.DoctorId && dto.CreatedBy != activeAppointment.CreatedBy)
                    {
                        await Clients.Caller.SendAsync("ReleaseRoomResult", new
                        {
                            Success = false,
                            Message = "You are not authorized to release this room"
                        });
                        return;
                    }

                    dto.id = activeAppointment.Id;
                    await _appointmentRepo.UpdateEndTimeAsync(dto);
                }

                // Change status to Available
                var room = await _context.Rooms.Include(r => r.Department).FirstOrDefaultAsync(r => r.Id == roomId);
                room!.Status = RoomStatus.Available;
                await _context.SaveChangesAsync();

                // Add outbox message for billing and email
                if (activeAppointment != null)
                {
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == activeAppointment.PatientId);
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == activeAppointment.DoctorId);
                    
                    var outboxMsg = new OutboxMessage
                    {
                        Type = "BillAndEmailForAppointment",
                        Payload = System.Text.Json.JsonSerializer.Serialize(new Safi.Dto.OutBoxDto.BillAndEmailForAppointmentDto
                        {
                            AppointmentId = activeAppointment.Id,
                            PatientId = activeAppointment.PatientId,
                            PatientName = patient?.Name ?? "Unknown",
                            PatientEmail = patient?.Email,
                            DoctorId = activeAppointment.DoctorId,
                            DoctorName = doctor?.Name ?? "Unknown",
                            DoctorEmail = doctor?.Email,
                            TotalAmount = 0, // Will be calculated by background service
                            RoomType = roomType,
                            RoomNumber = room.Number,
                            DepartmentName = room.Department?.Name ?? "Unknown",
                            StartTime = activeAppointment.StartTime ?? EgyptTime.Now,
                            EndTime = activeAppointment.EndTime ?? EgyptTime.Now
                        })
                    };
                    _context.OutboxMessages.Add(outboxMsg);
                    await _context.SaveChangesAsync();
                }


                await db.CommitAsync();
                var groupName = $"{roomType}s_{room.DepartmentId}";

                // Notify caller
                await Clients.Caller.SendAsync("ReleaseRoomResult", new
                {
                    Success = true,
                    Message = activeAppointment != null
                        ? "Room released and appointment finished successfully"
                        : "Room released successfully",
                    RoomId = roomId,
                    FinishedAppointmentId = activeAppointment?.Id
                });

                // Broadcast to group
                await Clients.Group(groupName).SendAsync("RoomStatusChanged", new
                {
                    RoomId = roomId,
                    RoomNumber = room.Number,
                    Status = RoomStatus.Available,
                    RoomType = roomType,
                    DepartmentId = room.DepartmentId,
                    Message = "Room is now available"
                });

                // If an appointment was finished, maybe broadcast a specific event
                if (activeAppointment != null)
                {
                    await Clients.Group(groupName).SendAsync("AppointmentFinished", new
                    {
                        RoomId = roomId,
                        RoomNumber = room.Number,
                        AppointmentId = activeAppointment.Id,
                        PatientId = activeAppointment.PatientId,
                        EndTime = EgyptTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                await db.RollbackAsync();
                await Clients.Caller.SendAsync("ReleaseRoomResult", new
                {
                    Success = false,
                    Message = $"Error releasing room: {ex.Message}"
                });
            }
        }
    }
}
