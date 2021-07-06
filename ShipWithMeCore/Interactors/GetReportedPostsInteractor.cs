using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IGetReportedPostsUseCase"/>.
    /// </summary>
    internal sealed class GetReportedPostsInteractor : IGetReportedPostsUseCase
    {
        private readonly IReportedPostRepository reportedPostRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reportedPostRepository">reported post repository</param>
        internal GetReportedPostsInteractor(IReportedPostRepository reportedPostRepository)
        {
            Validate.That(reportedPostRepository, nameof(reportedPostRepository)).IsNot(null);
            this.reportedPostRepository = reportedPostRepository;
        }

        /// <inheritdoc cref="IGetReportedPostsUseCase.Get(string)"/>
        public async Task<IEnumerable<ReportedPostEntity>> Get(string postId)
        {
            var reportedPosts = await reportedPostRepository.GetByPostId(postId);

            return reportedPosts;
        }
    }
}
