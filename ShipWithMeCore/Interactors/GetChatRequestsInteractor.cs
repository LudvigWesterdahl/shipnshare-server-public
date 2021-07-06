using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IGetChatRequestsUseCase"/>.
    /// </summary>
    internal sealed class GetChatRequestsInteractor : IGetChatRequestsUseCase
    {
        private readonly IChatRequestRepository chatRequestRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chatRequestRepository">chat request repository</param>
        internal GetChatRequestsInteractor(IChatRequestRepository chatRequestRepository)
        {
            this.chatRequestRepository = chatRequestRepository;
        }

        private static bool IsOpen(ChatRequestEntity chatRequest)
        {
            return chatRequest.Chat.Participants.Keys.Count() < 2;
        }

        private async Task<IEnumerable<ChatRequestEntity>> GetBy(
            Func<ChatRequestEntity, bool> predicate,
            long userId,
            bool onlyOpen)
        {
            var allChatRequests = await chatRequestRepository.GetByUserId(userId);

            var filteredChatRequests = allChatRequests
                .Where(predicate)
                .AsEnumerable();

            if (onlyOpen)
            {
                return filteredChatRequests
                    .Where(IsOpen)
                    .AsEnumerable();
            }

            return filteredChatRequests;
        }

        /// <inheritdoc cref="IGetChatRequestsUseCase.GetAll(long, bool)"/>
        public async Task<IEnumerable<ChatRequestEntity>> GetAll(long userId, bool onlyOpen)
        {
            return await GetBy(cr => true, userId, onlyOpen);
        }

        /// <inheritdoc cref="IGetChatRequestsUseCase.GetReceived(long, bool)"/>
        public async Task<IEnumerable<ChatRequestEntity>> GetReceived(long userId, bool onlyOpen)
        {
            return await GetBy(cr => cr.ToUser.Id.Equals(userId), userId, onlyOpen);
        }

        /// <inheritdoc cref="IGetChatRequestsUseCase.GetSent(long, bool)"/>
        public async Task<IEnumerable<ChatRequestEntity>> GetSent(long userId, bool onlyOpen)
        {
            return await GetBy(cr => cr.FromUser.Id.Equals(userId), userId, onlyOpen);
        }
    }
}
