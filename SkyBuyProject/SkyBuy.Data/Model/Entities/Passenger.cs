using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SkyBuy.Data.Model.Entities
{
    public class Passenger
    {
        [Key]
        public int Id { get; set; }

        public int BookingId { get; set; }
        public required Booking Booking { get; set; }

        // Passenger details
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [MaxLength(20)]
        public string? PassportNumber { get; set; }

        [MaxLength(3)]
        public string? Nationality { get; set; }  // ISO code

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [Required]
        public PassengerType Type { get; set; } = PassengerType.Adult;
        
        [Required]
        public Gender PersonGender { get; set; }


        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedAt { get; set; }

    }

    public enum PassengerType
    {
        Adult,
        Child,
        Infant
    }
    
    public enum Gender
    {
        Male,
        Female
    }

}
