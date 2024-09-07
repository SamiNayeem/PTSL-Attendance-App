using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class inital13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherAttendances_Attendance_AttendanceId",
                table: "OtherAttendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtherAttendances",
                table: "OtherAttendances");

            migrationBuilder.RenameTable(
                name: "OtherAttendances",
                newName: "OtherAttendance");

            migrationBuilder.RenameIndex(
                name: "IX_OtherAttendances_AttendanceId",
                table: "OtherAttendance",
                newName: "IX_OtherAttendance_AttendanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtherAttendance",
                table: "OtherAttendance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherAttendance_Attendance_AttendanceId",
                table: "OtherAttendance",
                column: "AttendanceId",
                principalTable: "Attendance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherAttendance_Attendance_AttendanceId",
                table: "OtherAttendance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtherAttendance",
                table: "OtherAttendance");

            migrationBuilder.RenameTable(
                name: "OtherAttendance",
                newName: "OtherAttendances");

            migrationBuilder.RenameIndex(
                name: "IX_OtherAttendance_AttendanceId",
                table: "OtherAttendances",
                newName: "IX_OtherAttendances_AttendanceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtherAttendances",
                table: "OtherAttendances",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherAttendances_Attendance_AttendanceId",
                table: "OtherAttendances",
                column: "AttendanceId",
                principalTable: "Attendance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
