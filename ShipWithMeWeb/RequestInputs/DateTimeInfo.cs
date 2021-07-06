using System;
using ShipWithMeWeb.Helpers;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class DateTimeInfo
    {
        [Date]
        public string DateTime { get; set; }
    }
}
