using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IAnswerChatRequestUseCase"/>.
    /// </summary>
    internal sealed class AnswerChatRequestInteractor : IAnswerChatRequestUseCase
    {
        private readonly IChatRequestRepository chatRequestRepository;

        private readonly IChatRepository chatRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chatRequestRepository">chat request repository</param>
        /// <param name="chatRepository">the chat repository</param>
        /// <param name="userRepository">the user repository</param>
        internal AnswerChatRequestInteractor(
            IChatRequestRepository chatRequestRepository,
            IChatRepository chatRepository)
        {
            this.chatRequestRepository = chatRequestRepository;
            this.chatRepository = chatRepository;
        }

        /// <inheritdoc cref="IAnswerChatRequestUseCase.Answer(string, long, bool)"/>
        public async Task<bool> Answer(string chatRequestId, long userId, bool accept)
        {
            var chatRequest = await chatRequestRepository.GetById(chatRequestId);

            if (chatRequest == null)
            {
                return false;
            }

            if (!chatRequest.ToUser.Id.Equals(userId))
            {
                return false;
            }

            var chat = await chatRepository.GetById(chatRequest.Chat.Id);

            if (chat.Participants.Keys.Select(u => u.Id).Contains(userId))
            {
                return false;
            }

            var updatedChat = await chatRepository.AddOrUpdateUser(chat.Id, userId, accept);

            if (updatedChat == null)
            {
                return false;
            }

            var newChatRequest = new ChatRequestEntity(
                chatRequest.Id,
                chatRequest.CreatedAt,
                chatRequest.FromUser,
                chatRequest.ToUser,
                updatedChat,
                accept);

            var updatedChatRequest = await chatRequestRepository.Update(newChatRequest);

            return updatedChatRequest;
        }
    }
}
