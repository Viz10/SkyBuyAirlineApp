using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkyBuy.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHashed = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AccountType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.CheckConstraint("CK_Account_LoginFields", "\r\n                        (\r\n                            (AccountType = 1 AND Email IS NOT NULL AND Username IS NULL) \r\n                            OR\r\n                            (AccountType = 2 AND Username IS NOT NULL)\r\n                        )\r\n                        ");
                });

            migrationBuilder.CreateTable(
                name: "AircraftTypes",
                columns: table => new
                {
                    ICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    IATA = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TypicalSeats = table.Column<int>(type: "int", nullable: false),
                    MaxSeats = table.Column<int>(type: "int", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftTypes", x => x.ICAO);
                });

            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    ICAO = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IATA = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryISO = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlines", x => x.ICAO);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    ICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    IATA = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryISO = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Timezone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Elevation = table.Column<int>(type: "int", nullable: false),
                    Lat = table.Column<double>(type: "float", nullable: false),
                    Long = table.Column<double>(type: "float", nullable: false),
                    IsInternational = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.ICAO);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FleetAircrafts",
                columns: table => new
                {
                    RegistrationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AircraftTypeICAO = table.Column<string>(type: "nvarchar(4)", nullable: false),
                    AirlineICAO = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetAircrafts", x => x.RegistrationNumber);
                    table.ForeignKey(
                        name: "FK_FleetAircrafts_AircraftTypes_AircraftTypeICAO",
                        column: x => x.AircraftTypeICAO,
                        principalTable: "AircraftTypes",
                        principalColumn: "ICAO",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FleetAircrafts_Airlines_AirlineICAO",
                        column: x => x.AirlineICAO,
                        principalTable: "Airlines",
                        principalColumn: "ICAO");
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OriginIATA = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DestinationIATA = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    OriginICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DestinationICAO = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TypicalDuration = table.Column<TimeSpan>(type: "time(0)", precision: 0, nullable: false),
                    IsInternational = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.CheckConstraint("CK_Route_NotSameAirport_IATA", "[OriginIATA] <> [DestinationIATA]");
                    table.CheckConstraint("CK_Route_NotSameAirport_ICAO", "[OriginICAO] <> [DestinationICAO]");
                    table.ForeignKey(
                        name: "FK_Routes_Airports_DestinationICAO",
                        column: x => x.DestinationICAO,
                        principalTable: "Airports",
                        principalColumn: "ICAO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routes_Airports_OriginICAO",
                        column: x => x.OriginICAO,
                        principalTable: "Airports",
                        principalColumn: "ICAO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlightServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    AirlineICAO = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    FlightNumber = table.Column<int>(type: "int", nullable: false),
                    FlightIATA = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FlightICAO = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightServices_Airlines_AirlineICAO",
                        column: x => x.AirlineICAO,
                        principalTable: "Airlines",
                        principalColumn: "ICAO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlightServices_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightServiceId = table.Column<int>(type: "int", nullable: false),
                    AircraftTypeICAO = table.Column<string>(type: "nvarchar(4)", nullable: false),
                    PreviousLegId = table.Column<int>(type: "int", nullable: true),
                    ScheduledDepartureTimeUTC = table.Column<TimeSpan>(type: "time", nullable: false),
                    ScheduledArrivalTimeUTC = table.Column<TimeSpan>(type: "time", nullable: false),
                    OperatingDays = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsAutoGenerated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSchedules", x => x.Id);
                    table.CheckConstraint("CK_Schedule_Dates", "[ValidUntil] > [ValidFrom]");
                    table.CheckConstraint("CK_Schedule_Price", "[BasePrice] >= 0");
                    table.ForeignKey(
                        name: "FK_FlightSchedules_AircraftTypes_AircraftTypeICAO",
                        column: x => x.AircraftTypeICAO,
                        principalTable: "AircraftTypes",
                        principalColumn: "ICAO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlightSchedules_FlightSchedules_PreviousLegId",
                        column: x => x.PreviousLegId,
                        principalTable: "FlightSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FlightSchedules_FlightServices_FlightServiceId",
                        column: x => x.FlightServiceId,
                        principalTable: "FlightServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightScheduleId = table.Column<int>(type: "int", nullable: false),
                    FleetAircraftRegistrationNumber = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    DepartureDateTimeUTC = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ArrivalDateTimeUTC = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalSeats = table.Column<int>(type: "int", nullable: false),
                    SeatsBooked = table.Column<int>(type: "int", nullable: false),
                    SeatsReserved = table.Column<int>(type: "int", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Terminal = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Gate = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ActualDepartureTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActualArrivalTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                    table.CheckConstraint("CK_Flight_Price_Positive", "[CurrentPrice] >= 0");
                    table.CheckConstraint("CK_Flight_Seats", "[SeatsBooked] + [SeatsReserved] <= [TotalSeats]");
                    table.ForeignKey(
                        name: "FK_Flights_FleetAircrafts_FleetAircraftRegistrationNumber",
                        column: x => x.FleetAircraftRegistrationNumber,
                        principalTable: "FleetAircrafts",
                        principalColumn: "RegistrationNumber");
                    table.ForeignKey(
                        name: "FK_Flights_FlightSchedules_FlightScheduleId",
                        column: x => x.FlightScheduleId,
                        principalTable: "FlightSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingReference = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    NumberOfPassengers = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.CheckConstraint("CK_Booking_PassengerCount", "[NumberOfPassengers] > 0 AND [NumberOfPassengers] <= 5");
                    table.CheckConstraint("CK_Booking_Price", "[TotalPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_Bookings_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PassportNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PersonGender = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.Id);
                    table.CheckConstraint("CK_Passenger_Age", "DATEDIFF(YEAR, [DateOfBirth], GETDATE()) >= 0");
                    table.ForeignKey(
                        name: "FK_Passengers_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AircraftTypes",
                columns: new[] { "ICAO", "FullName", "IATA", "Manufacturer", "MaxSeats", "TypicalSeats" },
                values: new object[,]
                {
                    { "A20N", "Airbus A320neo", "32N", "Airbus", 194, 180 },
                    { "A21N", "Airbus A321neo", "32Q", "Airbus", 244, 206 },
                    { "A320", "Airbus A320-200", "320", "Airbus", 186, 180 },
                    { "A321", "Airbus A321-200", "321", "Airbus", 220, 200 }
                });

            migrationBuilder.InsertData(
                table: "Airlines",
                columns: new[] { "ICAO", "Country", "CountryISO", "IATA", "Name" },
                values: new object[,]
                {
                    { "WMT", "Malta", "MLT", "W4", "Wizz Air Malta" },
                    { "WZZ", "Hungary", "HU", "W6", "Wizz Air Hungary" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0 AND [Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true,
                filter: "[IsDeleted] = 0 AND [Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AircraftTypes_IATA",
                table: "AircraftTypes",
                column: "IATA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AircraftTypes_Manufacturer",
                table: "AircraftTypes",
                column: "Manufacturer");

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_IATA",
                table: "Airlines",
                column: "IATA",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AccountId",
                table: "Bookings",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingReference",
                table: "Bookings",
                column: "BookingReference",
                unique: true,
                filter: "[IsDeleted] = 0 ");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FlightId",
                table: "Bookings",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_FleetAircrafts_AircraftTypeICAO",
                table: "FleetAircrafts",
                column: "AircraftTypeICAO");

            migrationBuilder.CreateIndex(
                name: "IX_FleetAircrafts_AirlineICAO",
                table: "FleetAircrafts",
                column: "AirlineICAO");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalDateTimeUTC",
                table: "Flights",
                column: "ArrivalDateTimeUTC");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureDateTimeUTC",
                table: "Flights",
                column: "DepartureDateTimeUTC");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_FleetAircraftRegistrationNumber",
                table: "Flights",
                column: "FleetAircraftRegistrationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_FlightScheduleId",
                table: "Flights",
                column: "FlightScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_AircraftTypeICAO",
                table: "FlightSchedules",
                column: "AircraftTypeICAO");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_FlightServiceId",
                table: "FlightSchedules",
                column: "FlightServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_OperatingDays",
                table: "FlightSchedules",
                column: "OperatingDays");

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

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_ScheduledDepartureTimeUTC",
                table: "FlightSchedules",
                column: "ScheduledDepartureTimeUTC");

            migrationBuilder.CreateIndex(
                name: "IX_FlightServices_AirlineICAO",
                table: "FlightServices",
                column: "AirlineICAO");

            migrationBuilder.CreateIndex(
                name: "IX_FlightServices_FlightIATA",
                table: "FlightServices",
                column: "FlightIATA",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightServices_FlightICAO",
                table: "FlightServices",
                column: "FlightICAO",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightServices_FlightNumber",
                table: "FlightServices",
                column: "FlightNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightServices_RouteId",
                table: "FlightServices",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_BookingId",
                table: "Passengers",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_Email",
                table: "Passengers",
                column: "Email",
                filter: "[Email] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_FirstName_LastName",
                table: "Passengers",
                columns: new[] { "FirstName", "LastName" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_PassportNumber",
                table: "Passengers",
                column: "PassportNumber",
                unique: true,
                filter: "[PassportNumber] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_Phone",
                table: "Passengers",
                column: "Phone",
                filter: "[Phone] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AccountId",
                table: "Profiles",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_FirstName_LastName",
                table: "Profiles",
                columns: new[] { "FirstName", "LastName" },
                filter: "[IsDeleted]=0");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Phone",
                table: "Profiles",
                column: "Phone",
                unique: true,
                filter: "[IsDeleted]=0");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationICAO",
                table: "Routes",
                column: "DestinationICAO");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginIATA_DestinationIATA",
                table: "Routes",
                columns: new[] { "OriginIATA", "DestinationIATA" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_OriginICAO",
                table: "Routes",
                column: "OriginICAO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passengers");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "FleetAircrafts");

            migrationBuilder.DropTable(
                name: "FlightSchedules");

            migrationBuilder.DropTable(
                name: "AircraftTypes");

            migrationBuilder.DropTable(
                name: "FlightServices");

            migrationBuilder.DropTable(
                name: "Airlines");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Airports");
        }
    }
}
