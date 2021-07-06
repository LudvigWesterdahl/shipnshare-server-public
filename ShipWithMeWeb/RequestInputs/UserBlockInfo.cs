using System;
using System.ComponentModel.DataAnnotations;
using ShipWithMeWeb.Helpers;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class UserBlockInfo
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Reason { get; set; }

        [Date]
        public string From { get; set; }

        [Date]
        public string To { get; set; }
    }
}
