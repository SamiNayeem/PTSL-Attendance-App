using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class ErrorLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CheckoutLatitude",
                table: "OtherAttendance",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CheckoutLongitude",
                table: "OtherAttendance",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CheckoutLatitude",
                table: "Attendance",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CheckoutLongitude",
                table: "Attendance",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckoutLatitude",
                table: "OtherAttendance");

            migrationBuilder.DropColumn(
                name: "CheckoutLongitude",
                table: "OtherAttendance");

            migrationBuilder.DropColumn(
                name: "CheckoutLatitude",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "CheckoutLongitude",
                table: "Attendance");
        }
    }
}
