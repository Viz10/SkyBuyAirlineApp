using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{
    public class Airline
    {
        [Key]
        [MaxLength(3)]
        public required string ICAO { get; set; }

        [MaxLength(2)]
        public required string IATA { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(100)]
        public required string Country { get; set; }

        [MaxLength(5)]
        public required string CountryISO { get; set; }  
            
        /// Navigation Prop.
        public ICollection<FlightService> FlightServices { get; set; } = new List<FlightService>();
        public ICollection<FleetAircraft> FleetAircrafts { get; set; } = new List<FleetAircraft>();
    }
}
