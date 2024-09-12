﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTSLAttendanceManager.Migrations
{
    /// <inheritdoc />
    public partial class initial113 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionInfo",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionInfo",
                table: "Users");
        }
    }
}
