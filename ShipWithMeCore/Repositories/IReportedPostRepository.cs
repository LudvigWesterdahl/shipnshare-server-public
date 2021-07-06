using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface IReportedPostRepository
    {
        Task Create(string postId, long userId, DateTime createdAt, string message);

        Task<IEnumerable<ReportedPostEntity>> GetByPostId(string postId);

        Task<IEnumerable<ReportedPostEntity>> GetByUserId(long userId);
    }
}
