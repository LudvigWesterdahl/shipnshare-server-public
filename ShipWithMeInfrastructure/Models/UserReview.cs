using System;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class UserReview
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Rating { get; set; }

        public string Message { get; set; }

        public long ReviewerId { get; set; }

        public User Reviewer { get; set; }

        public string PostId { get; set; }

        public Post Post { get; set; }

        public UserReviewEntity ToUserReviewEntity()
        {
            return new UserReviewEntity(Id, CreatedAt, Rating, Message, Reviewer.ToUserEntity(), Post.ToPostEntity());
        }
    }
}
