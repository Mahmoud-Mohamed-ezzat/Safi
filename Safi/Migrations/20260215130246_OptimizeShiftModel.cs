using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safi.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeShiftModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "End_Time",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropColumn(
                name: "Start_Time",
                table: "AssignRoomToDoctors");

            migrationBuilder.AddColumn<int>(
                name: "ShiftId",
                table: "AssignRoomToDoctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignRoomToDoctors_ShiftId",
                table: "AssignRoomToDoctors",
                column: "ShiftId");

            // SEED DEFAULT SHIFT AND UPDATE EXISTING RECORDS
            migrationBuilder.Sql("INSERT INTO Shifts (StartTime, EndTime) VALUES ('08:00', '16:00')");
            migrationBuilder.Sql("UPDATE AssignRoomToDoctors SET ShiftId = (SELECT TOP 1 Id FROM Shifts)");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignRoomToDoctors_Shifts_ShiftId",
                table: "AssignRoomToDoctors",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignRoomToDoctors_Shifts_ShiftId",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_AssignRoomToDoctors_ShiftId",
                table: "AssignRoomToDoctors");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "AssignRoomToDoctors");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "End_Time",
                table: "AssignRoomToDoctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Start_Time",
                table: "AssignRoomToDoctors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
