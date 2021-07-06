using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IGetChatRequestsUseCase
    {
        /// <summary>
        /// Returns all chat requests sent by or to the given user, open and closed.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="onlyOpen">if the request has been answered or not</param>
        /// <returns>the chat requests</returns>
        Task<IEnumerable<ChatRequestEntity>> GetAll(long userId, bool onlyOpen);

        /// <summary>
        /// Returns all chat requests sent by the given user.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="onlyOpen">if the request has been answered or not</param>
        /// <returns>the chat requests</returns>
        Task<IEnumerable<ChatRequestEntity>> GetSent(long userId, bool onlyOpen);

        /// <summary>
        /// Returns all chat requests sent to the given user.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="onlyOpen">if the request has been answered or not</param>
        /// <returns>the chat requests</returns>
        Task<IEnumerable<ChatRequestEntity>> GetReceived(long userId, bool onlyOpen);
    }
}
