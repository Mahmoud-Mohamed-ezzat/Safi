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
        public DbSet<User> Users {get;set;}
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Staff> Staffs { get; set; } 

        // Core entities
        public DbSet<Department> Departments { get; set; }
        public DbSet<Room> Rooms { get; set; }              // includes ICU & Emergency
        public DbSet<ICU> Icus { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }
        //public DbSet<Ambulance> Ambulances => Set<Ambulance>();

        // Appointments & Reservations
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<AppointmentToRoom> AppointmentToRooms { get; set; }
        public DbSet<AssignRoomToDoctor> AssignRoomToDoctors { get; set; }

        // Reports & extras
        public DbSet<ReportDoctorToPatient> ReportDoctorToPatients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } 
        public DbSet<TimeAvailableOfDoctor> TimeAvailableOfDoctors { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // User inheritance (TPH)
            builder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Patient>("Patient")
                .HasValue<Doctor>("Doctor")
                .HasValue<Staff>("Staff");

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

                r.HasOne(res => res.Patient)
                 .WithMany(p => p.Reservations)
                 .HasForeignKey(res => res.PatientId)
                 .OnDelete(DeleteBehavior.Cascade);

                r.HasOne(res => res.Doctor)
                 .WithMany(d => d.Reservations)
                 .HasForeignKey(res => res.DoctorId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // Room → Department
            builder.Entity<Room>()
                .HasOne(r => r.Department)
                .WithMany(d => d.Rooms)
                .HasForeignKey(r => r.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Roles

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp="1"
            },
                new IdentityRole
                {
                    Id = "2",
                    Name = "SubAdmin",
                    NormalizedName = "SUBADMIN",
                    ConcurrencyStamp= "2"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "Patient",
                    NormalizedName = "PATIENT",
                    ConcurrencyStamp="3"    
                },
                new IdentityRole
                {
                    Id = "4",
                    Name = "Doctor",
                    NormalizedName = "DOCTOR",
                    ConcurrencyStamp="4"
                },
                new IdentityRole
                {
                    Id = "5",
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    ConcurrencyStamp="5"
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
