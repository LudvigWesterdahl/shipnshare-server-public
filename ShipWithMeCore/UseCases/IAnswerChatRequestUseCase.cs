using System;
using System.Threading.Tasks;

namespace ShipWithMeCore.UseCases
{
    public interface IAnswerChatRequestUseCase
    {
        /// <summary>
        /// Replies to a request to join a chat.
        /// </summary>
        /// <param name="chatRequestId">the chat request id</param>
        /// <param name="userId">the user id</param>
        /// <param name="accept">true to accept, false to deny</param>
        /// <returns>true if the reply was successful, false otherwise</returns>
        Task<bool> Answer(string chatRequestId, long userId, bool accept);
    }
}
