using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IGetReportedPostsUseCase
    {
        /// <summary>
        /// Returns all reports made on a particular post.
        /// </summary>
        /// <param name="postId">the post id</param>
        /// <returns>all reports, might be empty</returns>
        Task<IEnumerable<ReportedPostEntity>> Get(string postId);
    }
}
