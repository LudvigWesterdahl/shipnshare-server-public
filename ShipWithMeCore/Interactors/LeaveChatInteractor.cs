using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="ILeaveChatUseCase"/>.
    /// </summary>
    internal sealed class LeaveChatInteractor : ILeaveChatUseCase
    {
        private readonly IChatRepository chatRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chatRepository">chat repository</param>
        internal LeaveChatInteractor(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        /// <inheritdoc cref="ILeaveChatUseCase.Leave(string, long)"/>
        public async Task<bool> Leave(string chatId, long userId)
        {
            var chat = await chatRepository.AddOrUpdateUser(chatId, userId, false);

            return chat != null;
        }
    }
}
