using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safi.Migrations
{
    /// <inheritdoc />
    public partial class EditRoomsToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignRoomToDoctors_AppointmentToRooms_AppointmentToRoomId",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropIndex(
                name: "IX_AssignRoomToDoctors_AppointmentToRoomId",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropColumn(
                name: "AppointmentToRoomId",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "AssignRoomToDoctors");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "AssignRoomToDoctors",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "End_Time",
                table: "AssignRoomToDoctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "AssignRoomToDoctors",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Start_Time",
                table: "AssignRoomToDoctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropColumn(
                name: "End_Time",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropColumn(
                name: "Start_Time",
                table: "AssignRoomToDoctors");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentToRoomId",
                table: "AssignRoomToDoctors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "AssignRoomToDoctors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_AssignRoomToDoctors_AppointmentToRoomId",
                table: "AssignRoomToDoctors",
                column: "AppointmentToRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignRoomToDoctors_AppointmentToRooms_AppointmentToRoomId",
                table: "AssignRoomToDoctors",
                column: "AppointmentToRoomId",
                principalTable: "AppointmentToRooms",
                principalColumn: "Id");
        }
    }
}
