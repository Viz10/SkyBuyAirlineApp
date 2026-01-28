using Microsoft.EntityFrameworkCore;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkyBuy.Data.Model.Entities
{
    public class FleetAircraft
    {
        [Key]
        [MaxLength(20)]
        public string RegistrationNumber { get; set; } = null!;

        /// FK
        public required string AircraftTypeICAO { get; set; }
        public required AircraftType AircraftType { get; set; }

        /// FK
        public string? AirlineICAO { get; set; }
        public Airline? Airline { get; set; }

        public bool IsActive { get; set; } = true;
   

        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}
