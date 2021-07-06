using System;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    public sealed class ReportedPostEntity
    {
        public PostEntity Post { get; }

        public UserEntity User { get; }

        public DateTime CreatedAt { get; }

        public string Message { get; }

        public ReportedPostEntity(PostEntity post, UserEntity user, DateTime createdAt, string message)
        {
            Validate.That(post, nameof(post)).IsNot(null);
            Post = post;

            Validate.That(user, nameof(user)).IsNot(null);
            User = user;

            Validate.That(createdAt, nameof(createdAt)).IsNotGreaterThan(DateTime.UtcNow);
            CreatedAt = createdAt;

            Validate.That(message, nameof(message)).IsNot(null);
            Message = message;
        }
    }
}
