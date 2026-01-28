using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{

    // Parent of the schedule table
    public class FlightService
    {
        [Key]
        public int Id { get; set; }

        // FK
        public int RouteId { get; set; } //id: 101 from CLJ TO FCO
        public Route Route { get; set; } = null!;

        public string AirlineICAO { get; set; } = null!; /// WZZ
        public Airline Airline { get; set; } = null!;

        /// DATA

        [Required]
        public int FlightNumber { get; set; } /// 3301 (clj fco every day at 5.30) .When it can overlap => 3303 add new flight number in that day different hour

        public string FlightIATA { get;  set; } = null!; ///W63301
        public string FlightICAO { get;  set; } = null!; /// WMT3301


        public ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
    }
}
