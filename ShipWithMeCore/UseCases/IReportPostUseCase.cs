using System.Threading.Tasks;

namespace ShipWithMeCore.UseCases
{
    public interface IReportPostUseCase
    {
        /// <summary>
        /// Reports a post and returns true if the report was successful and false if this user
        /// has already reported the highest version of the given post.
        /// </summary>
        /// <param name="postId">the id of the post to report</param>
        /// <param name="userId">the user reporting the post</param>
        /// <param name="message">the message</param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> Report(string postId, long userId, string message);
    }
}
