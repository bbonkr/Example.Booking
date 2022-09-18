using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example.Booking.Data.SqlServer.Migrations
{
    public partial class Initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BeforeEventBuffer = table.Column<int>(type: "int", nullable: true),
                    AfterEventBuffer = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    End = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_User_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointment_User_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserAvailableTimetable",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    End = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAvailableTimetable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAvailableTimetable_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserTimeTableOverride",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    End = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTimeTableOverride", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTimeTableOverride_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_FromUserId",
                table: "Appointment",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ToUserId",
                table: "Appointment",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAvailableTimetable_UserId",
                table: "UserAvailableTimetable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTimeTableOverride_UserId",
                table: "UserTimeTableOverride",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "UserAvailableTimetable");

            migrationBuilder.DropTable(
                name: "UserTimeTableOverride");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
