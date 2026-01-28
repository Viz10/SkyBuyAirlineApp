using SkyBuy.Common.Helpers;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    public class ScheduleDataForFlightsDTO
    {

        public int FlightScheduleId { get; set; }
        public FlightSchedule FlightSchedule { get; set; } = null!;


        public TimeSpan LocalDepartureTime { get; set; } /// For calculating datetimeoffset for dep/arr times
        public DaysOfWeek OperatingDays { get; set; }  /// DaysOfWeek.Monday | DaysOfWeek.Friday
        public DateOnly ValidFrom { get; set; } 
        public DateOnly ValidUntil { get; set; }
        public TimeSpan TypicalDuration { get; set; }
        public string OriginIATA { get; set; } = null!;


        public string AircraftTypeICAO { get; set; } = null!; /// to calculate nr of seats
        public decimal CurrentPrice { get; set; }

    }
}
