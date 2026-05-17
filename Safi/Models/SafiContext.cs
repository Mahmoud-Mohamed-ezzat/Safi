using Safi.Models;
// File: Data/SafiDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Safi.Models
{
    public class SafiContext : IdentityDbContext<User>
    {
        public SafiContext(DbContextOptions<SafiContext> options) : base(options) { }

        // Users
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Nurse> Nurses { get; set; }

        // Core entities
        public DbSet<Department> Departments { get; set; }
        public DbSet<Room> Rooms { get; set; }              // includes ICU & Emergency
        public DbSet<ICU> Icus { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }
        //public DbSet<Ambulance> Ambulances => Set<Ambulance>();

        // Appointments & Reservations
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<AppointmentToRoom> AppointmentToRooms { get; set; }
        public DbSet<AssignWorks> AssignWorks { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        // Reports & extras
        public DbSet<ReportDoctorToPatient> ReportDoctorToPatients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TimeAvailableOfDoctor> TimeAvailableOfDoctors { get; set; }
        public DbSet<Message> Messages { get; set; }
        // Extra
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Prices> Prices { get; set; }
        public DbSet<AppointmentPrice> AppointmentPrices { get; set; }

        public DbSet<Analysis> Analysis { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Global Query Filter for Users
            builder.Entity<User>().HasQueryFilter(u => !u.IsDeleted).HasIndex(u => u.Email).IsUnique();

            // User inheritance (TPH)
            builder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Patient>("Patient")
                .HasValue<Doctor>("Doctor")
                .HasValue<Staff>("Staff")
                .HasValue<Nurse>("Nurse");

            // Room inheritance (TPH)
            builder.Entity<Room>()
                .HasDiscriminator<string>("RoomType")
                .HasValue<Room>("NormalRoom")
                .HasValue<ICU>("Icu")
                .HasValue<Emergency>("Emergency");

            // Many-to-many Patient ↔ Department (automatic junction table)
            builder.Entity<Patient>()
        .HasMany(p => p.Departments)                    // ICollection<Department> in Patient
        .WithMany(d => d.Patients)                      // ICollection<Patient> in Department
        .UsingEntity<Dictionary<string, object>>(
            "PatientDepartment",                        // name of the junction table

            // Right side = Department → Patient
            right => right
                .HasOne<Department>()
                .WithMany()
                .HasForeignKey("DepartmentId")
                .OnDelete(DeleteBehavior.Restrict),     // FIXES cascade cycle

            // Left side = Patient → Department
            left => left
                .HasOne<Patient>()
                .WithMany()
                .HasForeignKey("PatientId")
                .OnDelete(DeleteBehavior.Cascade)
            );


            // Reservation relationships
            builder.Entity<Reservation>(r =>
            {
                r.HasKey(res => res.Id);
                r.HasIndex(res => res.Id).IsUnique();
                r.HasOne(res => res.Patient)
                 .WithMany(p => p.Reservations)
                 .HasForeignKey(res => res.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

                r.HasOne(res => res.Doctor)
                 .WithMany(d => d.Reservations)
                 .HasForeignKey(res => res.DoctorId)
                 .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<Bill>(r =>
            {
                r.HasKey(res => res.Id);
                r.HasIndex(res => res.Id).IsUnique();
                r.HasOne(res => res.Patient)
                 .WithMany(p => p.Bills)
                 .HasForeignKey(res => res.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Nurse>()
                .HasOne(n => n.Department)
                .WithMany() // Nurse has one Department, Department doesn't necessarily have a collection of Nurses in the model (I should check)
                .HasForeignKey(n => n.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


            // Room → Department
            builder.Entity<Room>()
                .HasOne(r => r.Department)
                .WithMany(d => d.Rooms)
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Room>()
                .HasIndex(r => r.Id).IsUnique();
            //Appointments
            builder.Entity<AppointmentToRoom>()
                .HasIndex(a => new { a.Id, a.PatientId, a.DoctorId, a.StartTime, a.EndTime }).IsUnique();
            //Prices
            builder.Entity<Prices>()
                .HasIndex(p => new { p.ServiceName, p.St_Date, p.End_Date, p.Is_Deleted });
            //Bill
            builder.Entity<Bill>()
                .HasIndex(a => a.Id).IsUnique();
            //AppointmentsPrices
            builder.Entity<AppointmentPrice>()
                .HasIndex(a => a.id).IsUnique();
            //Analysis
            builder.Entity<Analysis>()
                .HasIndex(a => a.Id).IsUnique();
            //Attendance
            builder.Entity<Attendance>()
                .HasIndex(a => a.Id).IsUnique();
            //AppointmentsToRooms
            builder.Entity<AppointmentToRoom>()
                .HasIndex(a => a.Id).IsUnique();
            //Reservation
            builder.Entity<Reservation>()
                .HasIndex(a => new { a.DoctorId, a.Id }).IsUnique();
            //Prices
            builder.Entity<Prices>()
                .HasIndex(a => new { a.ServiceName, a.St_Date, a.End_Date, a.Id }).IsUnique();
            //Bill
            builder.Entity<Bill>()
                .HasIndex(a => a.Id).IsUnique();
            //OutboxMessage
            builder.Entity<OutboxMessage>()
                .HasIndex(a => a.Id).IsUnique();
            // Seed Roles

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            },
                new IdentityRole
                {
                    Id = "2",
                    Name = "SubAdmin",
                    NormalizedName = "SUBADMIN",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "Patient",
                    NormalizedName = "PATIENT",
                    ConcurrencyStamp = "3"
                },
                new IdentityRole
                {
                    Id = "4",
                    Name = "Doctor",
                    NormalizedName = "DOCTOR",
                    ConcurrencyStamp = "4"
                },
                new IdentityRole
                {
                    Id = "5",
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    ConcurrencyStamp = "5"
                },
                new IdentityRole
                {
                    Id = "6",
                    Name = "Nurse",
                    NormalizedName = "NURSE",
                    ConcurrencyStamp = "6"
                }
                );
            // Seed Departments
            builder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Heart" },
                new Department { Id = 2, Name = "Kidney" },
                new Department { Id = 3, Name = "Liver" }
              );
        }
    }
}
