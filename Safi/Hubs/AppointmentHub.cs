using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Safi.Dto.AppointmentToRoom;
using Safi.Dto.EmailDto;
using Safi.Interfaces;
using Safi.Dto.ReportDoctorToPatientDto;
using Safi.Mapper;
using Safi.Models;

namespace Safi.Hubs
{
    public class AppointmentHub : Hub
    {
        private readonly SafiContext _context;
        private readonly IAppointmentToRoom _appointmentRepo;
        private readonly IAssignRoomToDoctor _assignRoomRepo;
        private readonly IEmailService _emailService;

        public AppointmentHub(SafiContext context, IAppointmentToRoom appointmentRepo, IAssignRoomToDoctor assignRoomRepo, IEmailService emailService)
        {
            _context = context;
            _appointmentRepo = appointmentRepo;
            _assignRoomRepo = assignRoomRepo;
            _emailService = emailService;
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
                        .Include(r => r.AssignRoomToDoctors!)
                            .ThenInclude(a => a.Doctor)
                        .Where(r => r.GetType() == typeof(Room) &&
                                    r.DepartmentId == departmentId &&
                                    r.Status == RoomStatus.Available)
                        .AsNoTracking()
                        .ToListAsync();

                    availableRooms = rooms.Select(r => r.ToAvailableRoomInfoDto(r.AssignRoomToDoctors?.ToList() ?? new List<AssignRoomToDoctor>())).ToList();
                }
                else if (roomType == "ICU")
                {
                    var icus = await _context.Icus
                        .Include(r => r.Department)
                        .Include(r => r.AssignRoomToDoctors!)
                            .ThenInclude(a => a.Doctor)
                        .Where(r => r.DepartmentId == departmentId && r.Status == RoomStatus.Available)
                        .AsNoTracking()
                        .ToListAsync();

                    availableRooms = icus.Select(r => r.ToAvailableRoomInfoDto(r.AssignRoomToDoctors?.ToList() ?? new List<AssignRoomToDoctor>())).ToList();
                }
                else if (roomType == "Emergency")
                {
                    var emergencies = await _context.Emergencies
                        .Include(r => r.Department)
                        .Include(r => r.AssignRoomToDoctors!)
                            .ThenInclude(a => a.Doctor)
                        .Where(r => r.DepartmentId == departmentId && r.Status == RoomStatus.Available)
                        .AsNoTracking()
                        .ToListAsync();

                    availableRooms = emergencies.Select(r => r.ToAvailableRoomInfoDto(r.AssignRoomToDoctors?.ToList() ?? new List<AssignRoomToDoctor>())).ToList();
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

                // Send email notification to doctor
                if (doctor != null && !string.IsNullOrEmpty(doctor.Email))
                {
                    var patientName = patient?.Name ?? "Unknown Patient";
                    await _emailService.SendEmailAsync(new SendEmailDto
                    {
                        ToEmail = doctor.Email,
                        Subject = "New Appointment Created",
                        Body = $@"
                            <h2>New Appointment Notification</h2>
                            <p>Dear Dr. {doctor.Name},</p>
                            <p>A new appointment has been created for you.</p>
                            <p><strong>Appointment Details:</strong></p>
                            <ul>
                                <li><strong>Patient:</strong> {patientName}</li>
                                <li><strong>Room:</strong> {roomType} #{room.Number}</li>
                                <li><strong>Department:</strong> {room.Department?.Name ?? "N/A"}</li>
                                <li><strong>Start Time:</strong> {appointment.StartTime:MMMM dd, yyyy hh:mm tt}</li>
                            </ul>
                            <p>Please be prepared for this appointment.</p>
                            <p>Best regards,<br/>Safi Hospital Team</p>"
                    });
                }

                var groupName = $"{roomType}s_{room.DepartmentId}";

                // Notify caller
                await Clients.Caller.SendAsync("AppointmentCreationResult", new
                {
                    Success = true,
                    Message = "Appointment created successfully",
                    Appointment = appointment
                });

                // Broadcast to group
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
        public async Task ReleaseRoom(int roomId, string roomType, CreateReportWhenPatientGetOutRoomDto dto)
        {
            var db = await _context.Database.BeginTransactionAsync();
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
                    await Clients.Caller.SendAsync("ReleaseRoomResult", new
                    {
                        Success = false,
                        Message = "Room not found"
                    });
                    return;
                }
                // Check for active appointment and close it
                var activeAppointment = await _appointmentRepo.GetActiveAppointmentByRoomIdAsync(roomId);

                if (activeAppointment != null)
                {
                    if (dto.CreatedBy != activeAppointment.DoctorId)
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
                room.Status = RoomStatus.Available;
                await _context.SaveChangesAsync();
                db.Commit();
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
                        EndTime = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                db.Rollback();
                await Clients.Caller.SendAsync("ReleaseRoomResult", new
                {
                    Success = false,
                    Message = $"Error releasing room: {ex.Message}"
                });
            }
        }
    }
}
