using System;
using System.Collections.Generic;
using System.Linq;
using ShipWithMeCore.Entities;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeWeb.Responses
{
    public sealed class ChatResponse
    {
        public string Id { get; set; }

        public string CreatedAt { get; set; }

        public string PostId { get; set; }

        public IDictionary<string, bool> Participants { get; set; }

        public IList<ChatMessageResponse> Messages { get; set; }

        public bool Closed { get; set; }
        
        private ChatResponse()
        {

        }

        public static ChatResponse NewChatResponse(ChatEntity chatEntity)
        {
            var chatResponse = new ChatResponse
            {
                Id = chatEntity.Id,
                CreatedAt = DateTimeUtils.ToString(chatEntity.CreatedAt),
                PostId = chatEntity.Post.Id,
                Participants = chatEntity.Participants
                    .ToDictionary(kv => kv.Key.UserName, kv => kv.Value),
                Messages = chatEntity.Messages
                    .OrderBy(cm => cm.CreatedAt)
                    .Select(cm => ChatMessageResponse.NewChatMessageResponse(cm))
                    .ToList(),
                Closed = chatEntity.Closed
            };

            return chatResponse;
        }
    }
}
