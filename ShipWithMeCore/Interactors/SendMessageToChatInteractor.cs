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
    /// Interactor for <see cref="ISendMessageToChatUseCase"/>.
    /// </summary>
    internal sealed class SendMessageToChatInteractor : ISendMessageToChatUseCase
    {
        private readonly IChatRepository chatRepository;

        internal SendMessageToChatInteractor(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        /// <inheritdoc cref="ISendMessageToChatUseCase.Send(string, long, string)"/>
        public async Task<ChatEntity> Send(string chatId, long userId, string message)
        {
            var chat = await chatRepository.GetById(chatId);

            if (chat == null || chat.Closed) {
                return null;
            }

            var participant = chat.Participants.Where(kv => kv.Key.Id == userId).FirstOrDefault();

            if (participant.Key == null || !participant.Value) {
                return null;
            }

            var updatedChat = await chatRepository.AddMessage(chatId, DateTime.UtcNow, message, userId);

            return updatedChat;
        }
    }
}
