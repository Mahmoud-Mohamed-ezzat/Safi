using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTimeAvailableOfDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Day",
                table: "TimeAvailableOfDoctors",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "Slots",
                table: "TimeAvailableOfDoctors",
                type: "int",
                nullable: false,
                defaultValue: 0);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "TimeAvailableOfDoctors");

            migrationBuilder.DropColumn(
                name: "Slots",
                table: "TimeAvailableOfDoctors");
        }
    }
}
