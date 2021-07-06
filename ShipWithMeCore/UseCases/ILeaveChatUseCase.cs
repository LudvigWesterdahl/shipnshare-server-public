using System;
using System.Threading.Tasks;

namespace ShipWithMeCore.UseCases
{
    public interface ILeaveChatUseCase
    {
        /// <summary>
        /// Leaves a given chat. If the user is the owner of the post that the chat is about, then this closes
        /// the chat for further conversation. Chat also becomes closed if the chat only had two participants and one
        /// of them leaves.
        /// </summary>
        /// <param name="chatId">the chat id</param>
        /// <param name="userId">the user id</param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> Leave(string chatId, long userId);
    }
}
