using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dental_Surgery.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PPS",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AwardingBody",
                table: "Dentists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Dentists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "attend",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PPS",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AwardingBody",
                table: "Dentists");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Dentists");

            migrationBuilder.DropColumn(
                name: "attend",
                table: "Appointments");
        }
    }
}
