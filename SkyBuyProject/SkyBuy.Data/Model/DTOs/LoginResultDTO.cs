using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    // from auth service to main dashboard
    public class LoginResultDTO
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public AccountWelcomeDataDTO? AccountData { get; set; }

    }
}
