using System;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    public sealed class UserReviewEntity
    {
        /// <summary>
        /// The id of this review.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The date and time the review was created.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// The rating of the review. Good (3), good but (2), bad but (1), bad (0).
        /// </summary>
        public int Rating { get; }

        /// <summary>
        /// Optional message, can be empty.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The user who created this review.
        /// </summary>
        public UserEntity Reviewer { get; }

        /// <summary>
        /// The post that the reviewer showed interest in when talking to that post owner.
        /// </summary>
        public PostEntity Post { get; }


        public UserReviewEntity(
            string id, DateTime createdAt, int rating, string message, UserEntity reviewer, PostEntity post)
        {
            Validate.That(id, nameof(id)).IsNot(null);
            Id = id;

            Validate.That(createdAt, nameof(createdAt)).IsNotGreaterThan(DateTime.UtcNow);
            CreatedAt = createdAt;

            Validate.That(rating, nameof(rating)).IsAny(0, 1, 2, 3);
            Rating = rating;

            Validate.That(message, nameof(message)).IsNot(null);
            Message = message;

            Validate.That(reviewer, nameof(reviewer)).IsNot(null);
            Reviewer = reviewer;

            Validate.That(post, nameof(post)).IsNot(null);
            Post = post;
        }
    }
}
