using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
     public class LoginCredentialsDTO
    {
        /// email / username
        public string AccountUniqueCredential { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsAdmin { get; set; }
    }
}
