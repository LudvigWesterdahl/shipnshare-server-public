using Microsoft.Extensions.Logging;
using ShipWithMeCore.Repositories;

namespace ShipWithMeInfrastructure.Repositories
{
    public static class RepositoryFactory
    {
        public static IChatRepository NewChatRepository(ILogger<IChatRepository> logger, MainDbContext mainDbContext)
        {
            return new ChatRepository(logger, mainDbContext);
        }

        public static IChatRequestRepository NewChatRequestRepository(
            ILogger<IChatRequestRepository> logger, MainDbContext mainDbContext)
        {
            return new ChatRequestRepository(logger, mainDbContext);
        }

        public static IInternalNotificationRepository NewInternalNotificationRepository(
            ILogger<IInternalNotificationRepository> logger, MainDbContext mainDbContext)
        {
            return new InternalNotificationRepository(logger, mainDbContext);
        }

        public static IPostRepository NewPostRepository(ILogger<IPostRepository> logger, MainDbContext mainDbContext)
        {
            return new PostRepository(logger, mainDbContext);
        }

        public static IReportedPostRepository NewReportedPostsRepository(
            ILogger<IReportedPostRepository> logger, MainDbContext mainDbContext)
        {
            return new ReportedPostRepository(logger, mainDbContext);
        }

        public static ITagRepository NewTagRepository(ILogger<ITagRepository> logger, MainDbContext mainDbContext)
        {
            return new TagRepository(logger, mainDbContext);
        }

        public static IUserRepository NewUserRepository(ILogger<IUserRepository> logger, MainDbContext mainDbContext)
        {
            return new UserRepository(logger, mainDbContext);
        }

        public static IUserReviewsRepository NewUserReviewRepository(
            ILogger<IUserReviewsRepository> logger, MainDbContext mainDbContext)
        {
            return new UserReviewRepository(logger, mainDbContext);
        }
    }
}
