using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{

    /// Public id : airline code  Flight Number + Departure Date W63301 + 2024-05-10 if same day flight  W6 3303 2024-05-10
    public class Flight
    {
        [Key]
        public int Id { get; set; }


        // FKs
        public int FlightScheduleId { get; set; }
        public FlightSchedule FlightSchedule { get; set; } = null!;

        public string? FleetAircraftRegistrationNumber { get; set; }
        public FleetAircraft? FleetAircraft { get; set; } // Nullable. Specific Fleet aircraft will be asigned later



        [Required]
        public DateTimeOffset DepartureDateTimeUTC { get; set; }  // 2024-12-25 06:30:00
        [Required]
        public DateTimeOffset ArrivalDateTimeUTC { get; set; }  // 2024-12-25 08:45:00


    
        [Required]
        public int TotalSeats { get; set; }  // 180 (from schedule)
        [Required]
        public int SeatsBooked { get; set; } = 0;  // How many sold
        [Required]
        public int SeatsReserved { get; set; } = 0;  // Temporarily held during checkout
        // Dynamic pricing
        public decimal CurrentPrice { get; set; }  // Changes as seats fill


        
        // Terminal/Gate (assigned closer to departure)
        [MaxLength(5)]
        public string? Terminal { get; set; }  // "1"
        [MaxLength(5)]
        public string? Gate { get; set; }  // "A12"
        public FlightStatus Status { get; set; } = FlightStatus.Scheduled;



        // Timestamps , in case of delay
        public DateTimeOffset? ActualDepartureTime { get; set; }
        public DateTimeOffset? ActualArrivalTime { get; set; }



        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;



        // Navigation
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
       
    }

    public enum FlightStatus
    {
        Scheduled,      // Normal state
        CheckInOpen,    // 24h before
        Boarding,       // At gate
        Departed,       // In air
        Landed,         // Arrived
        Delayed,        // Behind schedule
        Cancelled       // Not operating
    }
}
