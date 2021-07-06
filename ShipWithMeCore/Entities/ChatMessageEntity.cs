using System;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    public sealed class ChatMessageEntity
    {
        /// <summary>
        /// The id of this message.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The date and time the message was sent.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// The message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The user who sent the message.
        /// </summary>
        public UserEntity User { get; }

        public ChatMessageEntity(string id, DateTime createdAt, string message, UserEntity user)
        {
            Validate.That(id, nameof(id)).IsNot(null);
            Id = id;

            Validate.That(createdAt, nameof(createdAt)).IsNotGreaterThan(DateTime.UtcNow);
            CreatedAt = createdAt;

            Validate.That(message, nameof(message)).IsNot(null);
            Message = message;

            Validate.That(user, nameof(user)).IsNot(null);
            User = user;
        }
    }
}
