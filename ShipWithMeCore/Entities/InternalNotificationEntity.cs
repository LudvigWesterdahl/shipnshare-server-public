using System;
using System.Collections.Generic;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    public sealed class InternalNotificationEntity
    {
        public string Id { get; }

        public DateTime CreatedAt { get; }

        private readonly IDictionary<string, string> languageCodeMessages;

        public IDictionary<string, string> LanguageCodeMessages =>
            new Dictionary<string, string>(languageCodeMessages);

        private InternalNotificationEntity(
            string id, DateTime createdAt, IDictionary<string, string> languageCodeMessages)
        {
            Id = id;
            CreatedAt = createdAt;
            this.languageCodeMessages = new Dictionary<string, string>(languageCodeMessages);
        }

        public static InternalNotificationEntity Create(
            string id, DateTime createdAt, IDictionary<string, string> languageCodeMessages)
        {
            Validate.That(id, nameof(id)).IsNot(null);

            Validate.That(createdAt, nameof(createdAt)).IsNotGreaterThan(DateTime.UtcNow);

            Validate.That(languageCodeMessages, nameof(languageCodeMessages)).IsNot(null);
            foreach (var langugeCodeAndMessage in languageCodeMessages)
            {
                Validate.That(langugeCodeAndMessage.Key, nameof(langugeCodeAndMessage.Key)).IsNot(null);
                Validate.That(langugeCodeAndMessage.Key.Length, nameof(langugeCodeAndMessage.Key.Length)).IsNot(0);
                Validate.That(langugeCodeAndMessage.Value, nameof(langugeCodeAndMessage.Value)).IsNot(null);
                Validate.That(langugeCodeAndMessage.Value.Length, nameof(langugeCodeAndMessage.Value.Length)).IsNot(0);
            }

            return new InternalNotificationEntity(id, createdAt, languageCodeMessages);
        }
    }
}
