using System;
using System.Threading.Tasks;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IReportPostUseCase"/>.
    /// </summary>
    internal sealed class ReportPostInteractor : IReportPostUseCase
    {
        private readonly IReportedPostRepository reportedPostRepository;

        private readonly IPostRepository postRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reportedPostRepository">reported post repository</param>
        /// <param name="postRepository">post repository</param>
        internal ReportPostInteractor(IReportedPostRepository reportedPostRepository, IPostRepository postRepository)
        {
            Validate.That(reportedPostRepository, nameof(reportedPostRepository)).IsNot(null);
            this.reportedPostRepository = reportedPostRepository;

            Validate.That(postRepository, nameof(postRepository)).IsNot(null);
            this.postRepository = postRepository;
        }

        /// <inheritdoc cref="IReportPostUseCase.Report(string, long, string)"/>
        public async Task<bool> Report(string postId, long userId, string message)
        {
            Validate.That(message, nameof(message)).IsNot(null);
            Validate.That(message.Length, nameof(message.Length)).IsGreaterThan(0);

            var post = await postRepository.GetById(postId);

            var userReportedPosts = await reportedPostRepository.GetByUserId(userId);

            foreach (var userReportedPost in userReportedPosts)
            {
                if (userReportedPost.Post.Id.Equals(post.Id))
                {
                    // This user has already reported this post.
                    return false;
                }
            }

            await reportedPostRepository.Create(postId, userId, DateTime.UtcNow, message);
            return true;
        }
    }
}
