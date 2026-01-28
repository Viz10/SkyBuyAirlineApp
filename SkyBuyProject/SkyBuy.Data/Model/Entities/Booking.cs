using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{
    public class Booking
    {
        
        [Key]
        public int Id { get; set; }

        
        [MaxLength(10)]
        public required string BookingReference { get; set; }  // "ABC123" - unique code

        // Links
        public int FlightId { get; set; }
        public required Flight Flight { get; set; }

        // Person who bought tickets
        public int AccountId { get; set; }
        public required Account Account { get; set; }
        

        // Booking details
        [Required]
        public int NumberOfPassengers { get; set; }  // How many seats purchased
        public decimal TotalPrice { get; set; }  // Total amount paid


        // Status
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Timestamps
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        
        
        
        // Navigation
        public ICollection<Passenger> Passengers { get; set; } = new List<Passenger>();
       
        // public ICollection<SeatAssignment> SeatAssignments { get; set; }
        
    }

    public enum BookingStatus
    {
        Pending,        // Just created, awaiting payment
        Reserved,       // Temporarily held (15 min)
        Confirmed,      // Payment received
        CheckedIn,      // Passenger checked in
        Cancelled,      // User cancelled
        Expired         // Reservation timeout
    }
}
