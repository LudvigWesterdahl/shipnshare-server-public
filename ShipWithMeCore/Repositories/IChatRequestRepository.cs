using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface IChatRequestRepository
    {
        /// <summary>
        /// Returns the chat request with the given id.
        /// </summary>
        /// <param name="chatRequestId">the chat request id</param>
        /// <returns>the chat request</returns>
        Task<ChatRequestEntity> GetById(string chatRequestId);

        /// <summary>
        /// Returns all chat requests creatd by or sent to a given user.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>all chat requests</returns>
        Task<IEnumerable<ChatRequestEntity>> GetByUserId(long userId);

        /// <summary>
        /// Returns all chat requests sent in regards to a given chat.
        /// </summary>
        /// <param name="chatId">the chat id</param>
        /// <returns>all chat requests</returns>
        Task<IEnumerable<ChatRequestEntity>> GetByChatId(string chatId);

        /// <summary>
        /// Creates a new chat request.
        /// </summary>
        /// <param name="dateTime">the date and time</param>
        /// <param name="fromUserId">user who sent it</param>
        /// <param name="toUserId">user who is receiving it</param>
        /// <param name="chatId">the id of the chat</param>
        /// <param name="accepted">if the request was accepted or not</param>
        /// <returns>the created chat request</returns>
        Task<ChatRequestEntity> Create(
            DateTime dateTime, long fromUserId, long toUserId, string chatId, bool accepted);

        /// <summary>
        /// Updates the chat request.
        /// </summary>
        /// <param name="chatRequestEntity">the chat request</param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> Update(ChatRequestEntity chatRequestEntity);
    }
}
