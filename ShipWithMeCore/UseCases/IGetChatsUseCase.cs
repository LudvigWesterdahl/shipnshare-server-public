using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IGetChatsUseCase
    {
        /// <summary>
        /// Returns the chat by the given id if the user is a participant.
        /// </summary>
        /// <param name="chatId">the chat id</param>
        /// <param name="userId">the user id</param>
        /// <returns>the chat</returns>
        Task<ChatEntity> GetById(string chatId, long userId);

        /// <summary>
        /// Returns all chats this user is currently in.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>all chats</returns>
        Task<IEnumerable<ChatEntity>> GetAll(long userId);
    }
}
