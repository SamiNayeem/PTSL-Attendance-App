using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class LeaveApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeaveType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserWiseLeave",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalLeaves = table.Column<long>(type: "bigint", nullable: false),
                    CasualLeave = table.Column<long>(type: "bigint", nullable: false),
                    SickLeave = table.Column<long>(type: "bigint", nullable: false),
                    PaidLeave = table.Column<long>(type: "bigint", nullable: false),
                    TotalLeave = table.Column<long>(type: "bigint", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "LeaveApplication",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplyingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
                    UserWiseLeaveId = table.Column<long>(type: "bigint", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AddressDuringLeave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApprovedByAsignee = table.Column<bool>(type: "bit", nullable: false),
                    AgreedByAssigneeAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApprovedByProjectManager = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedByProjectManagerAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApprovedByHR = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedByHRAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveApplication_LeaveType_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveApplication_UserWiseLeave_UserWiseLeaveId",
                        column: x => x.UserWiseLeaveId,
                        principalTable: "UserWiseLeave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveApplication_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "PtslId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveApplication_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "PtslId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_AssignedTo",
                table: "LeaveApplication",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_LeaveTypeId",
                table: "LeaveApplication",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_UserId",
                table: "LeaveApplication",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_UserWiseLeaveId",
                table: "LeaveApplication",
                column: "UserWiseLeaveId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWiseLeave_UserId",
                table: "UserWiseLeave",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveApplication");

            migrationBuilder.DropTable(
                name: "LeaveType");

            migrationBuilder.DropTable(
                name: "UserWiseLeave");
        }
    }
}
