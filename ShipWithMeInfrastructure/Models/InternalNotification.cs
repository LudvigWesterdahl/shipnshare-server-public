using System;
using System.Collections.Generic;
using System.Linq;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class InternalNotification
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<InternalNotificationMessage> InternalNotificationMessages { get; set; }

        public InternalNotificationEntity ToInternalNotificationEntity()
        {
            var languageCodeMessages = InternalNotificationMessages
                .ToDictionary(inm => inm.LanguageCode, inm => inm.Message);

            return InternalNotificationEntity.Create(Id, CreatedAt, languageCodeMessages);
        }
    }
}
