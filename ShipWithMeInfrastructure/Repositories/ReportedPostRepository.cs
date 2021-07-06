using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure.Repositories
{
    internal sealed class ReportedPostRepository : IReportedPostRepository
    {
        private readonly MainDbContext mainDbContext;
        private readonly ILogger<IReportedPostRepository> logger;

        internal ReportedPostRepository(ILogger<IReportedPostRepository> logger, MainDbContext mainDbContext)
        {
            Validate.That(logger, nameof(logger)).IsNot(null);
            this.logger = logger;

            Validate.That(mainDbContext, nameof(mainDbContext)).IsNot(null);
            this.mainDbContext = mainDbContext;
        }

        /// <inheritdoc cref="IReportedPostRepository.Create(string, long, DateTime, string)"/>
        public async Task Create(string postId, long userId, DateTime createdAt, string message)
        {
            var highestVersionedPosts = mainDbContext.Posts.Where(p => p.Id == postId).ToList();

            var reportedPost = new ReportedPost
            {
                Id = RepositoryUtils.NewGuidString(),
                PostId = postId,
                UserId = userId,
                CreatedAt = createdAt,
                Message = message
            };

            await mainDbContext.ReportedPosts.AddAsync(reportedPost);
            await mainDbContext.SaveChangesAsync();
        }

        /// <inheritdoc cref="IReportedPostRepository.GetByPostId(string)"/>
        public Task<IEnumerable<ReportedPostEntity>> GetByPostId(string postId)
        {
            return Task.Run(() =>
            {
                var reportedPosts = mainDbContext.ReportedPosts
                    .Where(rp => rp.PostId == postId)
                    .Include(rp => rp.Post)
                        .ThenInclude(p => p.Tags)
                    .Include(rp => rp.User)
                    .AsEnumerable()
                    .Select(rp => rp.ToReportedPostEntity())
                    .AsEnumerable();

                return reportedPosts;
            });
        }

        /// <inheritdoc cref="IReportedPostRepository.GetByUserId(long)"/>
        public Task<IEnumerable<ReportedPostEntity>> GetByUserId(long userId)
        {
            return Task.Run(() =>
            {
                var reportedPosts = mainDbContext.ReportedPosts
                    .Where(rp => rp.UserId == userId)
                    .Include(rp => rp.Post)
                        .ThenInclude(p => p.Tags)
                    .Include(rp => rp.User)
                    .AsEnumerable()
                    .Select(rp => rp.ToReportedPostEntity())
                    .AsEnumerable();

                return reportedPosts;
            });
        }
    }
}
