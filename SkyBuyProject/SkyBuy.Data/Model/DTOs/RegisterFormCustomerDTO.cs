using System;

namespace SkyBuy.Models
{
    public class RegisterFormCustomerDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? ConfirmPassword { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}