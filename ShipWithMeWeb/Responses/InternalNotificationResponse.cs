using System;
using System.Collections.Generic;

namespace ShipWithMeWeb.Responses
{
    public class InternalNotificationResponse
    {
        public string CreatedAt { get; set; }

        public IDictionary<string, string> LanguageCodeMessages { get; set; }
    }
}
