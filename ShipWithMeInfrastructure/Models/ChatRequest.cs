using System;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class ChatRequest
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public long FromUserId { get; set; }

        public User FromUser { get; set; }

        public long ToUserId { get; set; }

        public User ToUser { get; set; }

        public string ChatId { get; set; }

        public Chat Chat { get; set; }

        public bool Accepted { get; set; }

        public ChatRequestEntity ToChatRequestEntity()
        {
            return new ChatRequestEntity(
                Id, CreatedAt, FromUser.ToUserEntity(), ToUser.ToUserEntity(), Chat.ToChatEntity(), Accepted);
        }
    }
}
