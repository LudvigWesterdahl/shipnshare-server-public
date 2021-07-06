using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class InternalNotificationInfo
    {
        [Required]
        public string LanguageCode { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
