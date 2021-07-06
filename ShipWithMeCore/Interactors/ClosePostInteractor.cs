using System.Threading.Tasks;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IClosePostUseCase"/>.
    /// </summary>
    internal sealed class ClosePostInteractor : IClosePostUseCase
    {
        private readonly IPostRepository postRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="postRepository">post repository</param>
        internal ClosePostInteractor(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        /// <inheritdoc cref="IClosePostUseCase.Close(string, long)"/>
        public async Task<bool> Close(string postId, long userId)
        {
            var post = await postRepository.GetById(postId);

            if (!post.Owner.Id.Equals(userId))
            {
                return false;
            }

            if (!post.Open)
            {
                return false;
            }

            var success = await postRepository.SetOpen(post.Id, false);

            return success;
        }
    }
}
