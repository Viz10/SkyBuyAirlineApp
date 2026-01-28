using SkyBuy.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
     public class RouteResultDTO
     { 
        public bool Found { get; set; }
        public string? ErrorMsg { get; set; }
        public List<PartialScheduleJsonDTO>? Routes { get; set; }
     }
}
