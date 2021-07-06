using System.Threading.Tasks;

namespace ShipWithMeCore.UseCases
{
    public interface IClosePostUseCase
    {
        /// <summary>
        /// Closes a post.
        /// </summary>
        /// <param name="postId">the post id</param>
        /// <param name="userId">the user id of the one closing the post</param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> Close(string postId, long userId);
    }
}
