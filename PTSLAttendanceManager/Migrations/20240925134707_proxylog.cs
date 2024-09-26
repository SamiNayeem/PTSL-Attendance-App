using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class proxylog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProxyLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProxyGiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AbsentPersonId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProxyLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProxyLogs_Users_AbsentPersonId",
                        column: x => x.AbsentPersonId,
                        principalTable: "Users",
                        principalColumn: "PtslId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProxyLogs_Users_ProxyGiverId",
                        column: x => x.ProxyGiverId,
                        principalTable: "Users",
                        principalColumn: "PtslId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProxyLogs_AbsentPersonId",
                table: "ProxyLogs",
                column: "AbsentPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProxyLogs_ProxyGiverId",
                table: "ProxyLogs",
                column: "ProxyGiverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProxyLogs");
        }
    }
}
