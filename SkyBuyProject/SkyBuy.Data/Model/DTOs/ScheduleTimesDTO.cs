using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Data.Model.DTOs
{
    public class ScheduleTimesDTO
    {
        public TimeSpan? ArrivalTimeUTC { get; set; }
        public TimeSpan? DepartureTimeUTC { get; set; }
        public TimeSpan? LocalDepartureTime { get; set; }
        public TimeSpan? LocalArrivalTime { get; set; }

        public TimeSpan? Duration {  get; set; }
        public double Distance;

        public bool OK { get; set; }
        public string? ErrorMsg { get; set; } = null!;

        public string? DepartureOffsetString { get; set; } = null!;
        public string? ArrivalOffsetString { get; set; } = null!;
        public bool IsArrivalNextDay { get; set; }
    }
}
