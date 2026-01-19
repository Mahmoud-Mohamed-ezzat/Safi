using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safi.Migrations
{
    /// <inheritdoc />
    public partial class EditReportDoctorToPatientModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationTime",
                table: "ReportDoctorToPatients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationTime",
                table: "ReportDoctorToPatients",
                type: "datetime2",
                nullable: true);
        }
    }
}
