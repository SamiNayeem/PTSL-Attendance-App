using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLeave3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserWiseLeaveId",
                table: "LeaveApplication",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "UserWiseLeave",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PendingEarnedLeave = table.Column<long>(type: "bigint", nullable: false),
                    PendingCasualLeave = table.Column<long>(type: "bigint", nullable: false),
                    PendingSickLeave = table.Column<long>(type: "bigint", nullable: false),
                    PendingMaternityLeave = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWiseLeave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWiseLeave_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "PtslId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_UserWiseLeaveId",
                table: "LeaveApplication",
                column: "UserWiseLeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWiseLeave_UserId",
                table: "UserWiseLeave",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveApplication_UserWiseLeave_UserWiseLeaveId",
                table: "LeaveApplication",
                column: "UserWiseLeaveId",
                principalTable: "UserWiseLeave",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveApplication_UserWiseLeave_UserWiseLeaveId",
                table: "LeaveApplication");

            migrationBuilder.DropTable(
                name: "UserWiseLeave");

            migrationBuilder.DropIndex(
                name: "IX_LeaveApplication_UserWiseLeaveId",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "UserWiseLeaveId",
                table: "LeaveApplication");
        }
    }
}
