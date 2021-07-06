using System;
using ShipWithMeCore.Entities;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeWeb.Responses
{
    public sealed class UserReviewResponse
    {
        public string CreatedAt { get; set; }

        public int Rating { get; set; }

        public string Message { get; set; }

        public string ReviewerUserName { get; set; }

        public string PostId { get; set; }

        public static UserReviewResponse NewUserReviewResponse(UserReviewEntity userReviewEntity)
        {
            return new UserReviewResponse {
                CreatedAt = DateTimeUtils.ToString(userReviewEntity.CreatedAt),
                Rating = userReviewEntity.Rating,
                Message = userReviewEntity.Message,
                ReviewerUserName = userReviewEntity.Reviewer.UserName,
                PostId = userReviewEntity.Post.Id
            };
        }
    }
}
