using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{
    public class Route
    {
        [Key]
        public int Id { get; set; }


        [MaxLength(4)]
        public required string OriginICAO { get; set; }
        public Airport Origin { get; set; } = null!;


        [MaxLength(4)]
        public required string DestinationICAO { get; set; }
        public Airport Destination { get; set; } = null!;


        public double DistanceKm { get; set; }  
        public TimeSpan TypicalDuration { get; set; }  


        public bool IsInternational { get; set; } = true;
        public bool IsActive { get; set; } = true;


        // Navigation
        public ICollection<FlightService> FlightServices { get; set; } = new List<FlightService>();
    }
}
