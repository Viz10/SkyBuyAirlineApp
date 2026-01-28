using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{
    // Create AircraftType table
    public class AircraftType
    {
        
        [Key]
        [MaxLength(4)]
        public required string ICAO { get; set; }  // "A320"
        
        [MaxLength(4)]
        public required string IATA { get; set; }  // "32A"

        [MaxLength(100)]
        public string? FullName { get; set; }  // "Airbus A320-200"

        public int TypicalSeats { get; set; }  // 180
        public int MaxSeats { get; set; }      // 186

        [MaxLength(20)]
        public required string Manufacturer { get; set; } // "Airbus"


        // Navigation Prop.
        public ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
        public ICollection<FleetAircraft> FleetAircrafts { get; set; } = new List<FleetAircraft>();
    }
}
