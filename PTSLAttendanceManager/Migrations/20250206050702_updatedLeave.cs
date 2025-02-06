using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class updatedLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalPMUserPtslId",
                table: "LeaveApplication",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedBy",
                table: "LeaveApplication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_ApprovalPMUserPtslId",
                table: "LeaveApplication",
                column: "ApprovalPMUserPtslId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveApplication_Users_ApprovalPMUserPtslId",
                table: "LeaveApplication",
                column: "ApprovalPMUserPtslId",
                principalTable: "Users",
                principalColumn: "PtslId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveApplication_Users_ApprovalPMUserPtslId",
                table: "LeaveApplication");

            migrationBuilder.DropIndex(
                name: "IX_LeaveApplication_ApprovalPMUserPtslId",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "ApprovalPMUserPtslId",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                table: "LeaveApplication");
        }
    }
}
