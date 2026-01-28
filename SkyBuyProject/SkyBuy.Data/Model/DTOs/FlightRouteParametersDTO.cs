using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    public class FlightRouteParametersDTO
    {
        public required string DepartureAirport_IATA { get; set; }
        public required string ArrivalAirport_IATA { get; set; }
        public string? Airline_IATA { get; set; }
        public string? AircraftTypeICAO { get; set; }

    }
}
