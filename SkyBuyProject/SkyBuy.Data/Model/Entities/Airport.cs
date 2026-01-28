using Microsoft.EntityFrameworkCore;
using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SkyBuy.Data.Model.Entities
{
    public class Airport
    {
        [Key]
        [MaxLength(4)]
        public string ICAO { get; set; } = null!; // PK
        
        [MaxLength(3)]
        public string? IATA { get; set; } 

        [MaxLength(200)]
        public string Name { get; set;} = null!;

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string Country { get; set; } = null!;

        [MaxLength(2)]
        public string CountryISO { get; set; } = null!;

        [MaxLength(50)]
        public string Timezone { get; set; } = null!;

        [MaxLength(50)]
        public string? Region { get; set; }


        public int Elevation { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }


        public bool IsInternational { get; set; }
        public bool IsActive { get; set; } = true;


        public ICollection<Route> OriginRoutes { get; set; } = new List<Route>();
        public ICollection<Route> DestinationRoutes { get; set; } = new List<Route>();
    }
}
