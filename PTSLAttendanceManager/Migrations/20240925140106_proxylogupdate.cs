﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class proxylogupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProxyLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProxyLogs");
        }
    }
}
