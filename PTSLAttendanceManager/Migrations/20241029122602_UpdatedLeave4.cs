using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLeave4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LeaveDurationId",
                table: "LeaveApplication",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "LeaveDuration",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveDuration", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveApplication_LeaveDurationId",
                table: "LeaveApplication",
                column: "LeaveDurationId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveApplication_LeaveDuration_LeaveDurationId",
                table: "LeaveApplication",
                column: "LeaveDurationId",
                principalTable: "LeaveDuration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveApplication_LeaveDuration_LeaveDurationId",
                table: "LeaveApplication");

            migrationBuilder.DropTable(
                name: "LeaveDuration");

            migrationBuilder.DropIndex(
                name: "IX_LeaveApplication_LeaveDurationId",
                table: "LeaveApplication");

            migrationBuilder.DropColumn(
                name: "LeaveDurationId",
                table: "LeaveApplication");
        }
    }
}
