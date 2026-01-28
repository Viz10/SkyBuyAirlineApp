using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    public class FlightSelectDatasDTO
    {
        public int AccountId { get; set; }
        public string OriginIATA { get; set; } = null!;
        public string DestinationIATA { get; set; } = null!;
        public DateOnly DepartureDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public bool IsRoundTrip { get; set; }
    }
}
