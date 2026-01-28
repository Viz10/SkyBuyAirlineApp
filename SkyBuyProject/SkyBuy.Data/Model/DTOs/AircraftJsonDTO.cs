using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{ 
    public class AircraftJsonDTO
    {
        public string hex { get; set; } = null!; /// unique code
        public string airline_icao { get; set; } = null!;
        public string icao { get; set; } = null!; /// A320

    }
}

