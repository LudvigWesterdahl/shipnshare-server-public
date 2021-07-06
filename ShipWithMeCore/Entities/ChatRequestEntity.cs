using System;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    public sealed class ChatRequestEntity
    {
        public string Id { get; }

        public DateTime CreatedAt { get; }

        public UserEntity FromUser { get; }

        public UserEntity ToUser { get; }

        public ChatEntity Chat { get; }

        public bool Accepted { get; }

        public ChatRequestEntity(
            string id, DateTime createdAt, UserEntity fromUser, UserEntity toUser, ChatEntity chat, bool accepted)
        {
            Validate.That(id, nameof(id)).IsNot(null);
            Id = id;

            Validate.That(createdAt, nameof(createdAt)).IsNotGreaterThan(DateTime.UtcNow);
            CreatedAt = createdAt;

            Validate.That(fromUser, nameof(fromUser)).IsNot(null);
            FromUser = fromUser;

            Validate.That(toUser, nameof(toUser)).IsNot(null);
            ToUser = toUser;

            Validate.That(chat, nameof(chat)).IsNot(null);
            Chat = chat;

            Accepted = accepted;
        }
    }
}
