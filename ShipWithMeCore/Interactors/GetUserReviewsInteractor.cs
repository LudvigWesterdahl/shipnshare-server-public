using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IGetUserReviewsUseCase"/>.
    /// </summary>
    internal sealed class GetUserReviewsInteractor : IGetUserReviewsUseCase
    {
        private readonly IUserReviewsRepository userReviewsRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userReviewsRepository">user reviews repository</param>
        internal GetUserReviewsInteractor(IUserReviewsRepository userReviewsRepository)
        {
            this.userReviewsRepository = userReviewsRepository;
        }

        /// <inheritdoc cref="IGetUserReviewsUseCase.GetAll(long)"/>
        public async Task<IEnumerable<UserReviewEntity>> GetAll(long userId)
        {
            var receivedUserReviews = await GetReceived(userId);

            var sentUserReviews = await GetSent(userId);

            return receivedUserReviews.Concat(sentUserReviews);

        }

        /// <inheritdoc cref="IGetUserReviewsUseCase.GetReceived(long)"/>
        public async Task<IEnumerable<UserReviewEntity>> GetReceived(long userId)
        {
            var receivedUserReviews = await userReviewsRepository.GetByPostOwnerUserId(userId);

            return receivedUserReviews;
        }

        /// <inheritdoc cref="IGetUserReviewsUseCase.GetSent(long)"/>
        public async Task<IEnumerable<UserReviewEntity>> GetSent(long userId)
        {
            var sentUserReviews = await userReviewsRepository.GetByReviewerUserId(userId);

            return sentUserReviews;
        }
    }
}
