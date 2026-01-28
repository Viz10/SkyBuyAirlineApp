using SkyBuy.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    public class AccountWelcomeDataDTO
    {
        public int Id { get; set; }
        public AccountType Type { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
    }
}
