using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    public class AirportJsonDTO  // for reading lowercase - matches JSON
    {
        public string icao { get; set; } = null!;
        public string? iata { get; set; }
        public string name { get; set; } = null!;
        public string? city { get; set; }
        public string? state { get; set; }
        public string country { get; set; } = null!;
        public int elevation { get; set; } 
        public double lat { get; set; } 
        public double lon { get; set; } 
        public string tz { get; set; } = null!;
    }
}
