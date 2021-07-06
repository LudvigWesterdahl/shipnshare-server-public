using System;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface ISendMessageToChatUseCase
    {
        /// <summary>
        /// Sends a message to the chat.
        /// </summary>
        /// <param name="chatId">the chat id</param>
        /// <param name="userId">the user id</param>
        /// <param name="message">the message</param>
        /// <returns>the chat with the added message</returns>
        Task<ChatEntity> Send(string chatId, long userId, string message);
    }
}
