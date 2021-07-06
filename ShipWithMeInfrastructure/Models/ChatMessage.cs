using System;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class ChatMessage
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public long FromUserId { get; set; }

        public User FromUser { get; set; }

        public string Message { get; set; }

        public string ChatId { get; set; }

        public Chat Chat { get; set; }

        public ChatMessageEntity ToChatMessageEntity()
        {
            return new ChatMessageEntity(Id, CreatedAt, Message, FromUser.ToUserEntity());
        }
    }
}
