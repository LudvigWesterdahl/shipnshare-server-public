using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface IChatRepository
    {
        /// <summary>
        /// Returns the chat with the given id.
        /// </summary>
        /// <param name="chatId">the chat id</param>
        /// <returns>the chat</returns>
        Task<ChatEntity> GetById(string chatId);

        /// <summary>
        /// Returns all chats the user is participating in.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>all chats</returns>
        Task<IEnumerable<ChatEntity>> GetByUserId(long userId);

        /// <summary>
        /// Adds a message to a chat.
        /// </summary>
        /// <param name="chatId">the chat id</param>
        /// <param name="createdAt">the date and time</param>
        /// <param name="message">the message</param>
        /// <param name="userId">the user who sent the message</param>
        /// <returns>the updated chat</returns>
        Task<ChatEntity> AddMessage(string chatId, DateTime createdAt, string message, long userId);


        /// <summary>
        /// Adds a user or updates the users active status.
        /// </summary>
        /// <param name="chatId">the chat id</param>
        /// <param name="userId">the user to add</param>
        /// <param name="active">if the user is active or not</param>
        /// <returns>the updated chat</returns>
        Task<ChatEntity> AddOrUpdateUser(string chatId, long userId, bool active);

        /// <summary>
        /// Creates a new chat.
        /// </summary>
        /// <param name="createdAt">the date and time</param>
        /// <param name="postId">the post id</param>
        /// <param name="participants">the participants and their active status</param>
        /// <returns>the created chat</returns>
        Task<ChatEntity> Create(DateTime createdAt, string postId, IDictionary<long, bool> participants);
    }
}
