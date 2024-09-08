using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class initial16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "OfficeLongitude",
                table: "UserConfigDtos",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "OfficeLatitude",
                table: "UserConfigDtos",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "Attendance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedOut",
                table: "Attendance",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "IsCheckedOut",
                table: "Attendance");

            migrationBuilder.AlterColumn<decimal>(
                name: "OfficeLongitude",
                table: "UserConfigDtos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "OfficeLatitude",
                table: "UserConfigDtos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
