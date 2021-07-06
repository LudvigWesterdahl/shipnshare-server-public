using ShipWithMeCore.ExternalServices;
using ShipWithMeCore.Interactors;
using ShipWithMeCore.Repositories;

namespace ShipWithMeCore.UseCases
{
    public static class UseCaseFactory
    {
        public static IAnswerChatRequestUseCase NewAnswerChatRequestUseCase(
            IChatRequestRepository chatRequestRepository, IChatRepository chatRepository)
        {
            return new AnswerChatRequestInteractor(chatRequestRepository, chatRepository);
        }

        public static IClosePostUseCase NewClosePostUseCase(IPostRepository postRepository)
        {
            return new ClosePostInteractor(postRepository);
        }

        public static ICreateInternalNotificationUseCase NewCreateInternalNotificationUseCase(
            IInternalNotificationRepository internalNotificationRepository)
        {
            return new CreateInternalNotificationInteractor(internalNotificationRepository);
        }

        public static ICreatePostUseCase NewCreatePostUseCase(IPostRepository postRepository)
        {
            return new CreatePostInteractor(postRepository);
        }

        public static IGetChatRequestsUseCase NewGetChatRequestsUseCase(
            IChatRequestRepository chatRequestRepository)
        {
            return new GetChatRequestsInteractor(chatRequestRepository);
        }

        public static IGetChatsUseCase NewGetChatsUseCase(IChatRepository chatRepository)
        {
            return new GetChatsInteractor(chatRepository);
        }

        public static IGetInternalNotificationsUseCase NewGetInternalNotificationsUseCase(
            IInternalNotificationRepository internalNotificationRepository)
        {
            return new GetInternalNotificationsInteractor(internalNotificationRepository);
        }

        public static IGetPostsUseCase NewGetPostsUseCase(
            IPostRepository postRepository, ILocationService locationService)
        {
            return new GetPostsInteractor(postRepository, locationService);
        }

        public static IGetReportedPostsUseCase NewGetReportedPostsUseCase(
            IReportedPostRepository reportedPostRepository)
        {
            return new GetReportedPostsInteractor(reportedPostRepository);
        }

        public static IGetUserReviewsUseCase NewGetUserReviewsUseCase(IUserReviewsRepository userReviewsRepository)
        {
            return new GetUserReviewsInteractor(userReviewsRepository);
        }

        public static IGetUserUseCase NewGetUserUseCase(IUserRepository userRepository)
        {
            return new GetUserInteractor(userRepository);
        }

        public static ILeaveChatUseCase NewLeaveChatUseCase(IChatRepository chatRepository)
        {
            return new LeaveChatInteractor(chatRepository);
        }

        public static IReportPostUseCase NewReportPostUseCase(
            IReportedPostRepository reportedPostRepository, IPostRepository postRepository)
        {
            return new ReportPostInteractor(reportedPostRepository, postRepository);
        }

        public static IRequestToChatUseCase NewRequestToChatUseCase(
            IChatRepository chatRepository,
            IChatRequestRepository chatRequestRepository,
            IPostRepository postRepository)
        {
            return new RequestToChatInteractor(chatRepository, chatRequestRepository, postRepository);
        }

        public static IReviewUserUseCase NewReviewUserUseCase(
            IUserReviewsRepository userReviewsRepository,
            IPostRepository postRepository,
            IChatRequestRepository chatRequestRepository)
        {
            return new ReviewUserInteractor(userReviewsRepository, postRepository, chatRequestRepository);
        }

        public static ISendMessageToChatUseCase NewSendMessageToChatUseCase(IChatRepository chatRepository)
        {
            return new SendMessageToChatInteractor(chatRepository);
        }
    }
}
