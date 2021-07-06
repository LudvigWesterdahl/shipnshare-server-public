using System;
using ShipWithMeCore.Entities;

namespace ShipWithMeWeb.Responses
{
    public sealed class AnswerChatRequestResponse
    {
        public bool Accepted { get; }

        public ChatRequestResponse ChatRequestResponse { get; }

        public ChatResponse ChatResponse { get; }

        private AnswerChatRequestResponse(
            bool accepted, ChatRequestResponse chatRequestResponse, ChatResponse chatResponse)
        {
            Accepted = accepted;
            ChatRequestResponse = chatRequestResponse;
            ChatResponse = chatResponse;

        }

        public static AnswerChatRequestResponse NewAnswerChatRequestResponse(ChatRequestEntity chatRequestEntity)
        {
            if (chatRequestEntity.Accepted)
            {
                return new AnswerChatRequestResponse(
                    true,
                    ChatRequestResponse.NewChatRequestResponse(chatRequestEntity),
                    ChatResponse.NewChatResponse(chatRequestEntity.Chat));
            }
            else
            {
                return new AnswerChatRequestResponse(
                    false,
                    ChatRequestResponse.NewChatRequestResponse(chatRequestEntity),
                    null);
            }

            
        }
    }
}
