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
    /// Interactor for <see cref="IReviewUserUseCase"/>.
    /// </summary>
    internal sealed class ReviewUserInteractor : IReviewUserUseCase
    {
        private readonly IUserReviewsRepository userReviewsRepository;

        private readonly IPostRepository postRepository;

        private readonly IChatRequestRepository chatRequestRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userReviewsRepository">user reviews repository</param>
        /// <param name="postRepository">post repository</param>
        /// <param name="chatRepository">chat repository</param>
        internal ReviewUserInteractor(
            IUserReviewsRepository userReviewsRepository,
            IPostRepository postRepository,
            IChatRequestRepository chatRequestRepository)
        {
            this.userReviewsRepository = userReviewsRepository;
            this.postRepository = postRepository;
            this.chatRequestRepository = chatRequestRepository;
        }

        private static bool HasConnectedWithPostOwner(string postId, IEnumerable<ChatRequestEntity> chatRequests)
        {
            var requests = chatRequests.ToList()
                .Select(cr => new {
                    PostId = cr.Chat.Post.Id,
                    PostOwner = cr.Chat.Post.Owner.Id,
                    Accepted = cr.Accepted
                })
                .ToList();

            foreach (var chatRequest in chatRequests) {
                if (chatRequest.Accepted && chatRequest.Chat.Post.Id.Equals(postId)) {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc cref="IReviewUserUseCase.Review(string, long)"/>
        public async Task<bool> CanReview(string postId, long reviewerUserId)
        {
            var post = await postRepository.GetById(postId);

            if (post == null) {
                return false;
            }

            if (post.Owner.Id.Equals(reviewerUserId)) {
                return false;
            }

            var reviewerReviews = await userReviewsRepository.GetByReviewerUserId(reviewerUserId);

            foreach (var review in reviewerReviews) {
                if (review.Post.Id.Equals(postId)) {
                    return false;
                }
            }

            var reviewerChatRequests = await chatRequestRepository.GetByUserId(reviewerUserId);

            return HasConnectedWithPostOwner(postId, reviewerChatRequests);
        }

        /// <inheritdoc cref="IReviewUserUseCase.Review(string, long, int, string)"/>
        public async Task<bool> Review(string postId, long reviewerUserId, int rating, string message)
        {
            var canReview = await CanReview(postId, reviewerUserId);

            if (!canReview) {
                return false;
            }

            if (rating < 0 || rating > 3) {
                return false;
            }

            if (message == null) {
                return false;
            }

            var userReview = await userReviewsRepository.Create(
                DateTime.UtcNow, rating, message, reviewerUserId, postId);

            return userReview != null;
        }
    }
}
