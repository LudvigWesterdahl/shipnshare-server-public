using System.Threading.Tasks;

namespace ShipWithMeCore.UseCases
{
    public interface IReviewUserUseCase
    {
        /// <summary>
        /// Returns true if the user is allowed to review the given post.
        /// </summary>
        /// <param name="postId">the post the review is in regards to</param>
        /// <param name="reviewerUserId">the reviewer user id</param>
        /// <returns>true if possible, false otherwise</returns>
        Task<bool> CanReview(string postId, long reviewerUserId);

        /// <summary>
        /// Leaves a review on attributed to the owner of a given post. 
        /// </summary>
        /// <param name="postId">the post the review is in regards to</param>
        /// <param name="reviewerUserId">the reviewer user id</param>
        /// <param name="rating">the rating, good (3), good but (2), bad but(1), bad (0)</param>
        /// <param name="message">the message, can be empty</param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> Review(string postId, long reviewerUserId, int rating, string message);
    }
}
