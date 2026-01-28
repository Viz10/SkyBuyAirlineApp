using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SkyBuy.Data.Model.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SkyBuy.Common.Helpers;

namespace SkyBuy.Data.Data
{
    public class SkyBuyContext : DbContext
    {

        public SkyBuyContext(DbContextOptions<SkyBuyContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; } /// table in DB
        public DbSet<Account> Accounts { get; set; }

        public DbSet<Airport> Airports { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<FleetAircraft> FleetAircrafts { get; set; }
        public DbSet<AircraftType> AircraftTypes { get; set; }

        public DbSet<Route> Routes { get; set; }
        public DbSet<FlightService> FlightServices { get; set; }
        public DbSet<FlightSchedule> FlightSchedules { get; set; }
        public DbSet<Flight> Flights { get; set; }
        
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>(entity =>
            {

                entity.HasQueryFilter(a => !a.IsDeleted); /// when querying , skip deleted rows

                entity /// make unique and also when deleted allow re-usage
                .HasIndex(a => a.Email) /// indexes
                .IsUnique()
                .HasFilter("[IsDeleted] = 0 AND [Email] IS NOT NULL");

                entity
                .HasIndex(a => a.Username)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0 AND [Username] IS NOT NULL"); // Username must be unique only among non-deleted accounts and not null

                /*
                 * The "Unique" rule only applies when there is actually a value. SQL will now ignore the NULL values when checking for duplicates,
                 * If the value is NULL, it wont put it in the index, and wontt check it for uniqueness.
                 */

                var customer = (int)AccountType.Customer; /// get int representation
                var admin = (int)AccountType.Admin;

                

                entity.ToTable(t => t.HasCheckConstraint( /// policy
                            "CK_Account_LoginFields",
                            $@"
                        (
                            (AccountType = {customer} AND Email IS NOT NULL AND Username IS NULL) 
                            OR
                            (AccountType = {admin} AND Username IS NOT NULL)
                        )
                        "
                        ));

            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasQueryFilter(p => !p.IsDeleted); ///EF Core automatically injects AND Account.IsDeleted = 0 into every query
                entity.HasIndex(p => p.Phone).IsUnique().HasFilter("[IsDeleted]=0");
                entity.HasIndex(p => new {p.FirstName,p.LastName}).HasFilter("[IsDeleted]=0");
            });

            modelBuilder.Entity<Airport>(entity =>
            {

                entity.HasQueryFilter(e => e.IsActive);

                entity.HasIndex(e => e.IATA)
                      .IsUnique()
                      .HasFilter("[IATA] IS NOT NULL AND [IsActive] = 1");
                
                entity.HasIndex(e => e.ICAO)
                      .IsUnique()
                      .HasFilter("[ICAO] IS NOT NULL AND [IsActive] = 1");

                entity.HasIndex(e => e.Name).HasFilter("[IsActive] = 1");

                entity.HasIndex(e => e.City).HasFilter("[IsActive] = 1");

                entity.HasIndex(e => e.CountryISO).HasFilter("[IsActive] = 1");

                entity.HasIndex(e => new { e.Lat, e.Long }).HasFilter("[IsActive] = 1");

            });

            modelBuilder.Entity<Airline>(entity =>
            {

                entity.HasIndex(e => e.IATA).IsUnique();
              
                entity.HasData(
                new Airline()
                {
                    ICAO = "WZZ",
                    IATA = "W6",
                    Name = "Wizz Air Hungary",
                    CountryISO = "HU",
                    Country = "Hungary"
                },
                new Airline()
                {
                    ICAO = "WMT",
                    IATA = "W4",
                    Name = "Wizz Air Malta",
                    CountryISO = "MLT",
                    Country = "Malta"
                });
            });

            

            modelBuilder.Entity<AircraftType>(entity =>
            {
                entity.HasIndex(a => a.IATA).IsUnique();
                entity.HasIndex(a => a.Manufacturer);

                /// INIT. FLEET TYPES OF AIRPLANE
                entity.HasData(
                    
                    new AircraftType
                    {
                        ICAO = "A320",
                        IATA = "320",
                        FullName = "Airbus A320-200",
                        TypicalSeats = 180,
                        MaxSeats = 186,
                        Manufacturer = "Airbus"
                    },

                    new AircraftType
                    {
                        ICAO = "A321",
                        IATA = "321",
                        FullName = "Airbus A321-200",
                        TypicalSeats = 200,
                        MaxSeats = 220,
                        Manufacturer = "Airbus"
                    },

                    new AircraftType
                    {
                        ICAO = "A20N",
                        IATA = "32N",
                        FullName = "Airbus A320neo",
                        TypicalSeats = 180,
                        MaxSeats = 194,
                        Manufacturer = "Airbus"
                    },

                    new AircraftType
                    {
                        ICAO = "A21N",
                        IATA = "32Q",
                        FullName = "Airbus A321neo",
                        TypicalSeats = 206,
                        MaxSeats = 244,
                        Manufacturer = "Airbus"
                    }
                );
            });

            modelBuilder.Entity<Booking>(entity =>
            {
               entity.HasQueryFilter(a => !a.IsDeleted);
               entity.HasIndex(a=>a.BookingReference).IsUnique().HasFilter("[IsDeleted] = 0 ");
               entity.ToTable(t => t.HasCheckConstraint("CK_Booking_Price", "[TotalPrice] >= 0"));
               entity.ToTable(t => t.HasCheckConstraint("CK_Booking_PassengerCount","[NumberOfPassengers] > 0 AND [NumberOfPassengers] <= 5"));
               entity.Property(a=>a.TotalPrice).HasColumnType("decimal(5,2)");

            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.HasQueryFilter(p => !p.IsDeleted);
                
                entity.HasIndex(a => new { a.FirstName, a.LastName })
                      .HasFilter("[IsDeleted] = 0");
                
                entity.HasIndex(a => a.Email)
                      .HasFilter("[Email] IS NOT NULL AND [IsDeleted] = 0");
              
                entity.HasIndex(a => a.Phone)
                      .HasFilter("[Phone] IS NOT NULL AND [IsDeleted] = 0");

                entity.HasIndex(a => a.PassportNumber)
                      .IsUnique()
                      .HasFilter("[PassportNumber] IS NOT NULL AND [IsDeleted] = 0");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Passenger_Age",
                    "DATEDIFF(YEAR, [DateOfBirth], GETDATE()) >= 0"));

            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasIndex(a => a.ArrivalDateTimeUTC);
                entity.HasIndex(a => a.DepartureDateTimeUTC);

                entity.ToTable(t => t.HasCheckConstraint("CK_Flight_Seats", "[SeatsBooked] + [SeatsReserved] <= [TotalSeats]"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Flight_Price_Positive", "[CurrentPrice] >= 0"));
                entity.Property(a => a.CurrentPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(a=>a.FlightSchedule)
                .WithMany(b=>b.Flights)
                .HasForeignKey(a=>a.FlightScheduleId)
                .HasPrincipalKey(b=>b.Id)
                .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.HasQueryFilter(e => e.IsActive);

                entity.ToTable(t => t.HasCheckConstraint("CK_Route_NotSameAirport_ICAO", "[OriginICAO] <> [DestinationICAO]"));

                entity.HasIndex(e => new { e.OriginICAO, e.DestinationICAO }).HasFilter("[IsActive] = 1");

                entity.Property(p => p.TypicalDuration).HasPrecision(0);
                entity.Property(p => p.DistanceKm).HasColumnType("decimal(18,2)"); 

                entity.HasOne(r => r.Origin)
                      .WithMany(a => a.OriginRoutes)
                      .HasForeignKey(r => r.OriginICAO)
                      .HasPrincipalKey(a => a.ICAO)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Destination)
                      .WithMany(a => a.DestinationRoutes)
                      .HasForeignKey(r => r.DestinationICAO)
                      .HasPrincipalKey(a => a.ICAO)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<FlightSchedule>(entity =>
            {
                entity.HasQueryFilter(e => e.IsActive);

                entity.HasIndex(e => e.OperatingDays).HasFilter("[IsActive] = 1");
                entity.HasIndex(e => e.LocalDepartureTime).HasFilter("[IsActive] = 1");

                entity.ToTable(t => t.HasCheckConstraint("CK_Schedule_Price", "[BasePrice] >= 0"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Schedule_Dates", "[ValidUntil] > [ValidFrom]"));

                entity.Property(a => a.BasePrice).HasColumnType("decimal(18,2)");

                entity.HasOne(s => s.PreviousLeg)
                .WithOne()
                .HasForeignKey<FlightSchedule>(s => s.PreviousLegId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.AircraftType)
                .WithMany(b => b.FlightSchedules)
                .HasForeignKey(a => a.AircraftTypeICAO)
                .HasPrincipalKey(b => b.ICAO)
                .OnDelete(DeleteBehavior.Restrict);
                
            });

            modelBuilder.Entity<FlightService>(entity =>
            {
                entity.HasIndex(a => a.FlightICAO).IsUnique();
                entity.HasIndex(a => a.FlightIATA).IsUnique();
                entity.HasIndex(a => a.FlightNumber).IsUnique();

                entity.HasOne(a => a.Airline)
                .WithMany(b => b.FlightServices)
                .HasForeignKey(a => a.AirlineICAO)
                .HasPrincipalKey(b => b.ICAO)
                .OnDelete(DeleteBehavior.Restrict);

            });

        }

    }
}
