using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyBuy.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRouteMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginIATA_DestinationIATA",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginICAO",
                table: "Routes");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Route_NotSameAirport_IATA",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules");

            migrationBuilder.DropColumn(
                name: "DestinationIATA",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "OriginIATA",
                table: "Routes");

            migrationBuilder.AlterColumn<int>(
                name: "PreviousLegId",
                table: "FlightSchedules",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginICAO_DestinationICAO",
                table: "Routes",
                columns: new[] { "OriginICAO", "DestinationICAO" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules",
                column: "PreviousLegId",
                unique: true,
                filter: "[PreviousLegId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_OriginICAO_DestinationICAO",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules");

            migrationBuilder.AddColumn<string>(
                name: "DestinationIATA",
                table: "Routes",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginIATA",
                table: "Routes",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

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
                name: "IX_Routes_OriginIATA_DestinationIATA",
                table: "Routes",
                columns: new[] { "OriginIATA", "DestinationIATA" },
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginICAO",
                table: "Routes",
                column: "OriginICAO");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Route_NotSameAirport_IATA",
                table: "Routes",
                sql: "[OriginIATA] <> [DestinationIATA]");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_PreviousLegId",
                table: "FlightSchedules",
                column: "PreviousLegId",
                unique: true);
        }
    }
}
