using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IGetUserReviewsUseCase
    {
        /// <summary>
        /// Returns all user reviews created by or for the given user.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>the user reviews</returns>
        Task<IEnumerable<UserReviewEntity>> GetAll(long userId);

        /// <summary>
        /// Returns all user reviews created by the given user.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>the user reviews</returns>
        Task<IEnumerable<UserReviewEntity>> GetSent(long userId);

        /// <summary>
        /// Returns all user reviews for the given user.
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <returns>the user reviews</returns>
        Task<IEnumerable<UserReviewEntity>> GetReceived(long userId);
    }
}
