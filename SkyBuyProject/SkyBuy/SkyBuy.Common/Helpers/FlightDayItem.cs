using System;
using System.Collections.Generic;
using System.Text;

namespace SkyBuy.Common.Helpers
{
    public class FlightDayItem
    {
        public string Name { get; set; } = null!;
        public bool IsChecked { get; set; }
        public int Value { get; set; }
    }

    [Flags]
    public enum DaysOfWeek
    {
        Monday = 1 << 0,    /// 1
        Tuesday = 1 << 1,   /// 2
        Wednesday = 1 << 2, /// 4
        Thursday = 1 << 3,  /// 8
        Friday = 1 << 4,/// 16
        Saturday = 1 << 5,  /// 32
        Sunday = 1 << 6     /// 64
    }

}
