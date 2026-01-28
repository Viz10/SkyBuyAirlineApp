using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SkyBuy.Data.Model.DTOs
{
    public class PartialScheduleJsonDTO
    {
        [JsonPropertyName("airline_iata")]
        public string AirlineIATA { get; set; } = null!;
        
        [JsonPropertyName("airline_icao")]
        public string AirlineICAO { get; set; } = null!;

        [JsonPropertyName("flight_iata")]
        public string FlightIATA { get; set; } = null!;

        [JsonPropertyName("flight_icao")]
        public string FlightICAO { get; set; } = null!;

        [JsonPropertyName("flight_number")]
        public string FlightNumber { get; set; } = null!;
        
    }
}
