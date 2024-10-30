using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLeave6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApprovalStatus",
                table: "LeaveApplication",
                newName: "ApprovalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_ApprovalStatusId",
                table: "LeaveApplication",
                column: "ApprovalStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveApplication_ApprovalStatus_ApprovalStatusId",
                table: "LeaveApplication",
                column: "ApprovalStatusId",
                principalTable: "ApprovalStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveApplication_ApprovalStatus_ApprovalStatusId",
                table: "LeaveApplication");

            migrationBuilder.DropIndex(
                name: "IX_LeaveApplication_ApprovalStatusId",
                table: "LeaveApplication");

            migrationBuilder.RenameColumn(
                name: "ApprovalStatusId",
                table: "LeaveApplication",
                newName: "ApprovalStatus");
        }
    }
}
