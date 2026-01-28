using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBuy.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsActiveMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginIATA_DestinationIATA",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_LocalDepartureTime",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_OperatingDays",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Airports_City",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_CountryISO",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_IATA",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_Lat_Long",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_Name",
                table: "Airports");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginIATA_DestinationIATA",
                table: "Routes",
                columns: new[] { "OriginIATA", "DestinationIATA" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_LocalDepartureTime",
                table: "FlightSchedules",
                column: "LocalDepartureTime",
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_OperatingDays",
                table: "FlightSchedules",
                column: "OperatingDays",
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_City",
                table: "Airports",
                column: "City",
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_CountryISO",
                table: "Airports",
                column: "CountryISO",
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_IATA",
                table: "Airports",
                column: "IATA",
                unique: true,
                filter: "[IATA] IS NOT NULL AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_ICAO",
                table: "Airports",
                column: "ICAO",
                unique: true,
                filter: "[ICAO] IS NOT NULL AND [IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_Lat_Long",
                table: "Airports",
                columns: new[] { "Lat", "Long" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_Name",
                table: "Airports",
                column: "Name",
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginIATA_DestinationIATA",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_LocalDepartureTime",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_OperatingDays",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Airports_City",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_CountryISO",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_IATA",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_ICAO",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_Lat_Long",
                table: "Airports");

            migrationBuilder.DropIndex(
                name: "IX_Airports_Name",
                table: "Airports");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginIATA_DestinationIATA",
                table: "Routes",
                columns: new[] { "OriginIATA", "DestinationIATA" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_LocalDepartureTime",
                table: "FlightSchedules",
                column: "LocalDepartureTime");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_OperatingDays",
                table: "FlightSchedules",
                column: "OperatingDays");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_City",
                table: "Airports",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_CountryISO",
                table: "Airports",
                column: "CountryISO");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_IATA",
                table: "Airports",
                column: "IATA",
                unique: true,
                filter: "[IATA] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Airports_Lat_Long",
                table: "Airports",
                columns: new[] { "Lat", "Long" });

            migrationBuilder.CreateIndex(
                name: "IX_Airports_Name",
                table: "Airports",
                column: "Name");
        }
    }
}
