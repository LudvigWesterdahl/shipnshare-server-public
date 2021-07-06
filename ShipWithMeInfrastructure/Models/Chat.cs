using System;
using System.Collections.Generic;
using System.Linq;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class Chat
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PostId { get; set; }

        public Post Post { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; }

        public ICollection<ChatMessage> ChatMessages { get; set; }

        public ICollection<ChatRequest> ChatRequests { get; set; }

        public ChatEntity ToChatEntity()
        {
            var participants = new Dictionary<UserEntity, bool>();

            foreach (var chatUser in ChatUsers)
            {
                participants[chatUser.User.ToUserEntity()] = chatUser.Active;
            }

            var chatMessages = ChatMessages.Select(cm => cm.ToChatMessageEntity()).ToList();

            var chatEntity = new ChatEntity(Id, CreatedAt, Post.ToPostEntity(), participants, chatMessages);

            return chatEntity;
        }
    }
}
