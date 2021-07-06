using System;
using ShipWithMeCore.Entities;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeWeb.Responses
{
    public sealed class ChatRequestResponse
    {
        public string Id { get; set; }

        public string CreatedAt { get; set; }

        public string ChatId { get; set; }

        public string PostId { get; set; }

        public string FromUserName { get; set; }

        public string ToUserName { get; set; }

        public bool Accepted { get; set; }

        public static ChatRequestResponse NewChatRequestResponse(ChatRequestEntity chatRequestEntity)
        {
            var chatRequestResponse = new ChatRequestResponse
            {
                Id = chatRequestEntity.Id,
                CreatedAt = DateTimeUtils.ToString(chatRequestEntity.CreatedAt),
                ChatId = chatRequestEntity.Chat.Id,
                PostId = chatRequestEntity.Chat.Post.Id,
                FromUserName = chatRequestEntity.FromUser.UserName,
                ToUserName = chatRequestEntity.ToUser.UserName,
                Accepted = chatRequestEntity.Accepted
            };

            return chatRequestResponse;
        }

    }
}
