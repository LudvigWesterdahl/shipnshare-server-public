using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface IUserReviewsRepository
    {
        Task<UserReviewEntity> GetById(string userReviewId);

        Task<IEnumerable<UserReviewEntity>> GetByReviewerUserId(long userId);

        Task<IEnumerable<UserReviewEntity>> GetByPostOwnerUserId(long userId);

        Task<UserReviewEntity> Create(
            DateTime createdAt, int rating, string message, long reviewerUserId, string postId);
    }
}
