using Microsoft.EntityFrameworkCore;
using SkyBuy.Common.Helpers;
using SkyBuy.Data.Data;
using SkyBuy.Data.Model.DTOs;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SkyBuy.Services
{
    public class AirplaneFlightService
    {
        private readonly IDbContextFactory<SkyBuyContext> _contextFactory;
        private readonly AirportsClient _airportsClient;

        public AirplaneFlightService(IDbContextFactory<SkyBuyContext> contextFactory, AirportsClient airportsClient)
        {
            _contextFactory = contextFactory;
            _airportsClient = airportsClient;    
        }


        public async Task<(bool,string?)> GenerateIndividualFlights(ScheduleDataForFlightsDTO schedule, SkyBuyContext dbContext)
        {
            try
            {
                var depTZTask = _airportsClient.GetTimezoneByCodeAsync("IATA", schedule.OriginIATA);
                var number_of_seats = _airportsClient.GetSeatsNumber(schedule.AircraftTypeICAO, "TypicalSeats");

                await Task.WhenAll(depTZTask, number_of_seats);

                var seatsNumber = await number_of_seats;
                var depTZ = await depTZTask;

                if (depTZ == null || seatsNumber == null) { return (false, "airportsClient error"); }

                var flightScheduleId = schedule.FlightScheduleId;
                var airportTz = TimeZoneInfo.FindSystemTimeZoneById(depTZ);
                var flightsToAdd = new List<Flight>();

                for (var date = schedule.ValidFrom; date <= schedule.ValidUntil; date = date.AddDays(1))
                {
                    var currentDayFlag = Convert(date.DayOfWeek);

                    if (schedule.OperatingDays.HasFlag(currentDayFlag))
                    {
                        /// The "Bridge" is now the specific date we are generating
                        DateTime reference = date.ToDateTime(TimeOnly.MinValue);
                        DateTime localDep = DateTime.SpecifyKind(reference.Add(schedule.LocalDepartureTime), DateTimeKind.Unspecified);
            
                        /// This lookup handles the March/October jump automatically
                        TimeSpan currentOffset = airportTz.GetUtcOffset(localDep); 
            
                        var flight = new Flight
                        {
                            /// Absolute UTC moment for THIS specific day
                            DepartureDateTimeUTC = new DateTimeOffset(localDep, currentOffset).ToUniversalTime(),
                            ArrivalDateTimeUTC = new DateTimeOffset(localDep, currentOffset).ToUniversalTime().Add(schedule.TypicalDuration),
                            FlightScheduleId = flightScheduleId,
                            TotalSeats = seatsNumber.Value,
                            CurrentPrice = schedule.CurrentPrice,
                        };
                        flightsToAdd.Add(flight);
                    }
                }

                if (flightsToAdd.Any())
                {
                    
                await dbContext.Flights.AddRangeAsync(flightsToAdd);
                await dbContext.SaveChangesAsync();

                Debug.WriteLine("All flights inserted successfully!");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Main Error: {ex.Message}");

                // Walk down the stack of inner exceptions to find the actual DB error
                Exception? inner = ex.InnerException;
                while (inner != null)
                {
                    sb.AppendLine($"--> Inner Error: {inner.Message}");
                    inner = inner.InnerException;
                }

                return (false, sb.ToString());
            }
        }
        private DaysOfWeek Convert(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => DaysOfWeek.Monday,    // 1
                DayOfWeek.Tuesday => DaysOfWeek.Tuesday,   // 2
                DayOfWeek.Wednesday => DaysOfWeek.Wednesday, // 4
                DayOfWeek.Thursday => DaysOfWeek.Thursday,  // 8
                DayOfWeek.Friday => DaysOfWeek.Friday,    // 16
                DayOfWeek.Saturday => DaysOfWeek.Saturday,  // 32
                DayOfWeek.Sunday => DaysOfWeek.Sunday,    // 64
                _ => throw new ArgumentOutOfRangeException()
            };
        }

    }
}
