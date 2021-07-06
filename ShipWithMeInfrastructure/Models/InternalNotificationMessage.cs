using System;
namespace ShipWithMeInfrastructure.Models
{
    public sealed class InternalNotificationMessage
    {
        public string LanguageCode { get; set; }

        public string InternalNotificationId { get; set; }

        public InternalNotification InternalNotification { get; set; }

        public string Message { get; set; }
    }
}
