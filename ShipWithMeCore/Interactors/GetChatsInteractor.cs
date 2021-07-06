using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IGetChatsUseCase"/>.
    /// </summary>
    internal sealed class GetChatsInteractor : IGetChatsUseCase
    {
        private readonly IChatRepository chatRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chatRepository">chat repository</param>
        internal GetChatsInteractor(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        /// <summary>
        /// Returns true if the chat is open and the user has not left.
        /// </summary>
        /// <param name="chat">the chat</param>
        /// <param name="userId">the user id</param>
        /// <returns>true if open and user is active, false otherwise</returns>
        private static bool HasActiveUser(ChatEntity chat, long userId)
        {
            var activeParticipant = chat.Participants
                .Where(kv => kv.Key.Id.Equals(userId))
                .Select(kv => kv.Value)
                .First();

            return activeParticipant;
        }

        /// <summary>
        /// Returns true if the user is one of the participants.
        /// </summary>
        /// <param name="chat">the chat</param>
        /// <param name="userId">the user id</param>
        /// <returns>true if open and user is active, false otherwise</returns>
        private static bool HasUser(ChatEntity chat, long userId)
        {
            var activeParticipant = chat.Participants
                .Where(kv => kv.Key.Id.Equals(userId))
                .Select(kv => kv.Key)
                .FirstOrDefault();

            return activeParticipant != null;
        }

        /// <inheritdoc cref="IGetChatsUseCase.GetById(string, long)"/>
        public async Task<ChatEntity> GetById(string chatId, long userId)
        {
            var userChat = await chatRepository.GetById(chatId);

            if (!HasUser(userChat, userId))
            {
                return null;
            }

            return userChat;
        }

        /// <inheritdoc cref="IGetChatsUseCase.GetAll(long)"/>
        public async Task<IEnumerable<ChatEntity>> GetAll(long userId)
        {
            var userchats = await chatRepository.GetByUserId(userId);

            var activeNonClosedChats = userchats
                .Where(c => HasUser(c, userId))
                .AsEnumerable();

            return activeNonClosedChats;
        }
    }
}
