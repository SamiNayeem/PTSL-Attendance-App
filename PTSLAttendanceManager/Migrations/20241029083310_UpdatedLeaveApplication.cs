using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLeaveApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "AgreedByAssigneeAt",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "IsApprovedByAsignee",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "UserWiseLeaveId",
                table: "LeaveApplication");

            migrationBuilder.AddColumn<long>(
                name: "TotalLeaveDays",
                table: "LeaveType",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateOnly>(
                name: "FromDate",
                table: "LeaveApplication",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "LeaveApplication",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ToDate",
                table: "LeaveApplication",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalLeaveDays",
                table: "LeaveType");

            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "LeaveApplication");

            migrationBuilder.AddColumn<DateTime>(
                name: "AgreedByAssigneeAt",
                table: "LeaveApplication",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByAsignee",
                table: "LeaveApplication",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                    CasualLeave = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PaidLeave = table.Column<long>(type: "bigint", nullable: false),
                    SickLeave = table.Column<long>(type: "bigint", nullable: false),
                    TotalLeave = table.Column<long>(type: "bigint", nullable: false),
                    TotalLeaves = table.Column<long>(type: "bigint", nullable: false)
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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
