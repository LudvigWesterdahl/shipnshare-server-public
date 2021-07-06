using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IRequestToChatUseCase"/>.
    /// </summary>
    internal sealed class RequestToChatInteractor : IRequestToChatUseCase
    {
        private readonly IChatRepository chatRepository;

        private readonly IChatRequestRepository chatRequestRepository;

        private readonly IPostRepository postRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chatRepository">chat repository</param>
        /// <param name="chatRequestRepository">chat request repository</param>
        /// <param name="postRepository">post repository</param>
        internal RequestToChatInteractor(
            IChatRepository chatRepository,
            IChatRequestRepository chatRequestRepository,
            IPostRepository postRepository)
        {
            this.chatRepository = chatRepository;
            this.chatRequestRepository = chatRequestRepository;
            this.postRepository = postRepository;
        }

        /// <inheritdoc cref="IRequestToChatUseCase.Request(string, long)"/>
        public async Task<ChatRequestEntity> Request(string postId, long userId)
        {
            var post = await postRepository.GetById(postId);

            if (post.Owner.Id == userId)
            {
                return null;
            }

            var userChats = await chatRepository.GetByUserId(userId);

            foreach (var userChat in userChats)
            {
                if (userChat.Post.Id.Equals(post.Id))
                {
                    return null;
                }
            }

            var chat = await chatRepository.Create(DateTime.UtcNow, post.Id, new Dictionary<long, bool>()
            {
                {userId, true}
            });

            var chatRequest = await chatRequestRepository.Create(
                DateTime.UtcNow, userId, post.Owner.Id, chat.Id, false);

            return chatRequest;
        }
    }
}
