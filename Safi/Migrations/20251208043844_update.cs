using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Safi.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiredOn",
                table: "RefreshTokens",
                newName: "ExpiresOn");

            migrationBuilder.AddColumn<string>(
                name: "History",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "History",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ExpiresOn",
                table: "RefreshTokens",
                newName: "ExpiredOn");
        }
    }
}
