using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBuy.Data.Migrations
{
    /// <inheritdoc />
    public partial class LocalDepTimeMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_ScheduledArrivalTimeUTC",
                table: "FlightSchedules");

            migrationBuilder.DropColumn(
                name: "ScheduledArrivalTimeUTC",
                table: "FlightSchedules");

            migrationBuilder.RenameColumn(
                name: "ScheduledDepartureTimeUTC",
                table: "FlightSchedules",
                newName: "LocalDepartureTime");

            migrationBuilder.RenameIndex(
                name: "IX_FlightSchedules_ScheduledDepartureTimeUTC",
                table: "FlightSchedules",
                newName: "IX_FlightSchedules_LocalDepartureTime");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousLegId",
                table: "FlightSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules",
                column: "PreviousLegId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules");

            migrationBuilder.RenameColumn(
                name: "LocalDepartureTime",
                table: "FlightSchedules",
                newName: "ScheduledDepartureTimeUTC");

            migrationBuilder.RenameIndex(
                name: "IX_FlightSchedules_LocalDepartureTime",
                table: "FlightSchedules",
                newName: "IX_FlightSchedules_ScheduledDepartureTimeUTC");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousLegId",
                table: "FlightSchedules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ScheduledArrivalTimeUTC",
                table: "FlightSchedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules",
                column: "PreviousLegId",
                unique: true,
                filter: "[PreviousLegId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_ScheduledArrivalTimeUTC",
                table: "FlightSchedules",
                column: "ScheduledArrivalTimeUTC");
        }
    }
}
