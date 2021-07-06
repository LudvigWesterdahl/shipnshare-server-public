using System;
using ShipWithMeCore.Entities;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeWeb.Responses
{
    public class ChatMessageResponse
    {
        public string UserName { get; set; }

        public string CreatedAt { get; set; }

        public string Message { get; set; }

        public static ChatMessageResponse NewChatMessageResponse(ChatMessageEntity chatMessageEntity)
        {
            return new ChatMessageResponse
            {
                UserName = chatMessageEntity.User.UserName,
                CreatedAt = DateTimeUtils.ToString(chatMessageEntity.CreatedAt),
                Message = chatMessageEntity.Message
            };
        }
    }
}
