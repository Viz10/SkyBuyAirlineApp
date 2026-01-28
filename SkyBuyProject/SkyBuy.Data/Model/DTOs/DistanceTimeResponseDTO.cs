using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    public class DistanceTimeResponseDTO
    {
        public double? DistanceKm { get; set; }
        public string? Duration { get; set; }
        public bool Ok { get; set; } 
        public string? ErrorMsg { get; set; }
    }
}
