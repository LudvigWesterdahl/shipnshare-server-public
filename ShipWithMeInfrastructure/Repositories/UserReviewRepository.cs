using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure.Repositories
{
    internal sealed class UserReviewRepository : IUserReviewsRepository
    {
        private readonly ILogger<IUserReviewsRepository> logger;
        private readonly MainDbContext mainDbContext;

        internal UserReviewRepository(ILogger<IUserReviewsRepository> logger, MainDbContext mainDbContext)
        {
            Validate.That(logger, nameof(logger)).IsNot(null);
            this.logger = logger;
            Validate.That(mainDbContext, nameof(mainDbContext)).IsNot(null);
            this.mainDbContext = mainDbContext;
        }

        private IEnumerable<UserReviewEntity> UserReviews(Expression<Func<UserReview, bool>> predicate)
        {
            return mainDbContext.UserReviews
                .Where(predicate)
                .Include(ur => ur.Reviewer)
                .Include(ur => ur.Post)
                    .ThenInclude(p => p.Owner)
                .Include(ur => ur.Post)
                    .ThenInclude(p => p.Tags)
                .AsEnumerable()
                .Select(ur => ur.ToUserReviewEntity());
        }

        private Task<IEnumerable<UserReviewEntity>> UserReviewsAsync(Expression<Func<UserReview, bool>> predicate)
        {
            return Task.Run(() =>
            {
                return UserReviews(predicate);
            });
        }

        private UserReviewEntity UserReview(string userReviewId)
        {
            return UserReviews(ur => ur.Id == userReviewId).First();
        }

        private Task<UserReviewEntity> UserReviewAsync(string userReviewId)
        {
            return Task.Run(() =>
            {
                return UserReview(userReviewId);
            });
        }

        public async Task<UserReviewEntity> Create(DateTime createdAt, int rating, string message, long reviewerUserId, string postId)
        {
            var userReview = new UserReview
            {
                Id = RepositoryUtils.NewGuidString(),
                CreatedAt = createdAt,
                Rating = rating,
                Message = message,
                ReviewerId = reviewerUserId,
                PostId = postId,
            };

            await mainDbContext.UserReviews.AddAsync(userReview);
            await mainDbContext.SaveChangesAsync();

            return UserReview(userReview.Id);

        }

        public Task<UserReviewEntity> GetById(string userReviewId)
        {
            return UserReviewAsync(userReviewId);
        }

        public Task<IEnumerable<UserReviewEntity>> GetByPostOwnerUserId(long userId)
        {
            return UserReviewsAsync(ur => ur.Post.OwnerId == userId);
        }

        public Task<IEnumerable<UserReviewEntity>> GetByReviewerUserId(long userId)
        {
            return UserReviewsAsync(ur => ur.ReviewerId == userId);
        }
    }
}
