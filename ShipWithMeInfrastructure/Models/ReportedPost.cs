using System;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class ReportedPost
    {
        public string Id { get; set; }

        public string PostId { get; set; }

        public Post Post { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Message { get; set; }

        public ReportedPostEntity ToReportedPostEntity()
        {
            return new ReportedPostEntity(Post.ToPostEntity(), User.ToUserEntity(), CreatedAt, Message);
        }
    }
}
