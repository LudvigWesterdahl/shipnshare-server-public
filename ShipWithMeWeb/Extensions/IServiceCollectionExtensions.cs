using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.ExternalServices;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;
using ShipWithMeInfrastructure;
using ShipWithMeInfrastructure.Models;
using ShipWithMeInfrastructure.Repositories;
using ShipWithMeInfrastructure.Services;

namespace ShipWithMeWeb.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    internal static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all the dependencies from the Infrastructure project.
        /// </summary>
        /// <param name="services">the services instance</param>
        internal static void AddInfrastructure(this IServiceCollection services, string mainConnectionString)
        {
            services.AddDbContextPool<MainDbContext>(options =>
            {
                options.UseSqlServer(mainConnectionString,
                    b => b.MigrationsAssembly(typeof(MainDbContext).Assembly.GetName().Name));

                options.EnableSensitiveDataLogging(true);
            });

            services.AddIdentity<User, IdentityRole<long>>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<MainDbContext>()
                .AddDefaultTokenProviders();

            /*
            services.AddScoped<IEmailService>(sp => {

                var smtpOptions = sp.GetRequiredService<IOptions<SmtpOptions>>();

                return new EmailService(smtpOptions.Value.Port, smtpOptions.Value.From);
            });
            */
            //services.AddMailKit

            services.AddScoped<IEmailService, EmailService>();



            services.AddScoped<ILocationService, LocationService>();

            services.AddRepository<IChatRepository>(RepositoryFactory.NewChatRepository);

            services.AddRepository<IChatRequestRepository>(RepositoryFactory.NewChatRequestRepository);

            services.AddRepository<IInternalNotificationRepository>(
                RepositoryFactory.NewInternalNotificationRepository);

            services.AddRepository<IPostRepository>(RepositoryFactory.NewPostRepository);

            services.AddRepository<IReportedPostRepository>(RepositoryFactory.NewReportedPostsRepository);

            services.AddRepository<ITagRepository>(RepositoryFactory.NewTagRepository);

            services.AddRepository<IUserRepository>(RepositoryFactory.NewUserRepository);

            services.AddRepository<IUserReviewsRepository>(RepositoryFactory.NewUserReviewRepository);
        }

        /// <summary>
        /// Adds all the dependencies from the Core project.
        /// </summary>
        /// <param name="services">the services instance</param>
        internal static void AddCore(this IServiceCollection services)
        {
            services.AddScopedProvider<IAnswerChatRequestUseCase,
                IChatRequestRepository,
                IChatRepository>(UseCaseFactory.NewAnswerChatRequestUseCase);

            services.AddScopedProvider<IClosePostUseCase,
                IPostRepository>(UseCaseFactory.NewClosePostUseCase);

            services.AddScopedProvider<ICreateInternalNotificationUseCase,
                IInternalNotificationRepository>(UseCaseFactory.NewCreateInternalNotificationUseCase);

            services.AddScopedProvider<ICreatePostUseCase, IPostRepository>(UseCaseFactory.NewCreatePostUseCase);

            services.AddScopedProvider<IGetChatRequestsUseCase,
                IChatRequestRepository>(UseCaseFactory.NewGetChatRequestsUseCase);

            services.AddScopedProvider<IGetChatsUseCase,
                IChatRepository>(UseCaseFactory.NewGetChatsUseCase);

            services.AddScopedProvider<IGetInternalNotificationsUseCase,
                IInternalNotificationRepository>(UseCaseFactory.NewGetInternalNotificationsUseCase);

            services.AddScopedProvider<IGetPostsUseCase,
                IPostRepository,
                ILocationService>(UseCaseFactory.NewGetPostsUseCase);

            services.AddScopedProvider<IGetReportedPostsUseCase,
                IReportedPostRepository>(UseCaseFactory.NewGetReportedPostsUseCase);

            services.AddScopedProvider<IGetUserReviewsUseCase,
                IUserReviewsRepository>(UseCaseFactory.NewGetUserReviewsUseCase);

            services.AddScopedProvider<IGetUserUseCase,
                IUserRepository>(UseCaseFactory.NewGetUserUseCase);

            services.AddScopedProvider<ILeaveChatUseCase,
                IChatRepository>(UseCaseFactory.NewLeaveChatUseCase);

            services.AddScopedProvider<IReportPostUseCase,
                IReportedPostRepository,
                IPostRepository>(UseCaseFactory.NewReportPostUseCase);

            services.AddScopedProvider<IRequestToChatUseCase,
                IChatRepository,
                IChatRequestRepository,
                IPostRepository>(UseCaseFactory.NewRequestToChatUseCase);

            services.AddScopedProvider<IReviewUserUseCase,
                IUserReviewsRepository,
                IPostRepository,
                IChatRequestRepository>(UseCaseFactory.NewReviewUserUseCase);

            services.AddScopedProvider<ISendMessageToChatUseCase,
                IChatRepository>(UseCaseFactory.NewSendMessageToChatUseCase);
        }

        /// <summary>
        /// Adds a scoped service using
        /// <see cref="AddScopedProvider{TService, D1, D2}(IServiceCollection, Func{D1, D2, TService})"/>
        /// where D1 is <see cref="ILogger{TCategoryName}"/> and D2 is <see cref="MainDbContext"/> and TService
        /// is the repository to add.
        /// </summary>
        /// <typeparam name="TRepository">the type of the repository</typeparam>
        /// <param name="services">the services</param>
        /// <param name="provider">the service provider</param>
        /// <returns>this <see cref="IServiceCollection"/> for chaining</returns>
        internal static IServiceCollection AddRepository<TRepository>(
            this IServiceCollection services, Func<ILogger<TRepository>, MainDbContext, TRepository> provider)
            where TRepository : class
        {
            return services.AddScopedProvider(provider);
        }

        #region AddScopedProvider

        /// <summary>
        /// Adds a scoped service as produced by the given provider on the <see cref="IServiceCollection"/>
        /// without any dependencies.
        /// </summary>
        /// <typeparam name="TService">the service type</typeparam>
        /// <param name="services">the services</param>
        /// <returns>this <see cref="IServiceCollection"/> for chaining</returns>
        /// <exception cref="NullReferenceException">If the provider returned null.</exception>
        internal static IServiceCollection AddScopedProvider<TService>(
            this IServiceCollection services, Func<TService> provider) where TService : class
        {
            return services.AddScoped(sp =>
            {
                return provider.Invoke() ?? throw new NullReferenceException();
            });
        }

        /// <summary>
        /// Adds a scoped service as produced by the given provider on the <see cref="IServiceCollection"/>
        /// by first retrieving the dependencies from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">the service type</typeparam>
        /// <typeparam name="D1">the type of the first dependency</typeparam>
        /// <param name="services">the services</param>
        /// <param name="provider">the service provider</param>
        /// <returns>this <see cref="IServiceCollection"/> for chaining</returns>
        /// <exception cref="InvalidOperationException">If services were not found.</exception>
        /// <exception cref="NullReferenceException">If the provider returned null.</exception>
        internal static IServiceCollection AddScopedProvider<TService, D1>(
            this IServiceCollection services, Func<D1, TService> provider) where TService : class
        {
            return services.AddScoped(sp =>
            {
                var d1 = sp.GetRequiredService<D1>();
                return provider.Invoke(d1) ?? throw new NullReferenceException();
            });
        }

        /// <summary>
        /// Adds a scoped service as produced by the given provider on the <see cref="IServiceCollection"/>
        /// by first retrieving the dependencies from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">the service type</typeparam>
        /// <typeparam name="D1">the type of the first dependency</typeparam>
        /// <typeparam name="D2">the type of the second dependency</typeparam>
        /// <param name="services">the services</param>
        /// <param name="provider">the service provider</param>
        /// <returns>this <see cref="IServiceCollection"/> for chaining</returns>
        /// <exception cref="InvalidOperationException">If services were not found.</exception>
        /// <exception cref="NullReferenceException">If the provider returned null.</exception>
        internal static IServiceCollection AddScopedProvider<TService, D1, D2>(
            this IServiceCollection services, Func<D1, D2, TService> provider) where TService : class
        {
            return services.AddScoped(sp =>
            {
                var d1 = sp.GetRequiredService<D1>();
                var d2 = sp.GetRequiredService<D2>();
                return provider.Invoke(d1, d2) ?? throw new NullReferenceException();
            });
        }

        /// <summary>
        /// Adds a scoped service as produced by the given provider on the <see cref="IServiceCollection"/>
        /// by first retrieving the dependencies from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">the service type</typeparam>
        /// <typeparam name="D1">the type of the first dependency</typeparam>
        /// <typeparam name="D2">the type of the second dependency</typeparam>
        /// <typeparam name="D3">the type of the third dependency</typeparam>
        /// <param name="services">the services</param>
        /// <param name="provider">the service provider</param>
        /// <returns>this <see cref="IServiceCollection"/> for chaining</returns>
        /// <exception cref="InvalidOperationException">If services were not found.</exception>
        /// <exception cref="NullReferenceException">If the provider returned null.</exception>
        internal static IServiceCollection AddScopedProvider<TService, D1, D2, D3>(
            this IServiceCollection services, Func<D1, D2, D3, TService> provider) where TService : class
        {
            return services.AddScoped(sp =>
            {
                var d1 = sp.GetRequiredService<D1>();
                var d2 = sp.GetRequiredService<D2>();
                var d3 = sp.GetRequiredService<D3>();
                return provider.Invoke(d1, d2, d3) ?? throw new NullReferenceException();
            });
        }

        /// <summary>
        /// Adds a scoped service as produced by the given provider on the <see cref="IServiceCollection"/>
        /// by first retrieving the dependencies from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">the service type</typeparam>
        /// <typeparam name="D1">the type of the first dependency</typeparam>
        /// <typeparam name="D2">the type of the second dependency</typeparam>
        /// <typeparam name="D3">the type of the third dependency</typeparam>
        /// <typeparam name="D4">the type of the fourth dependency</typeparam>
        /// <param name="services">the services</param>
        /// <param name="provider">the service provider</param>
        /// <returns>this <see cref="IServiceCollection"/> for chaining</returns>
        /// <exception cref="InvalidOperationException">If services were not found.</exception>
        /// <exception cref="NullReferenceException">If the provider returned null.</exception>
        internal static IServiceCollection AddScopedProvider<TService, D1, D2, D3, D4>(
            this IServiceCollection services, Func<D1, D2, D3, D4, TService> provider) where TService : class
        {
            return services.AddScoped(sp =>
            {
                var d1 = sp.GetRequiredService<D1>();
                var d2 = sp.GetRequiredService<D2>();
                var d3 = sp.GetRequiredService<D3>();
                var d4 = sp.GetRequiredService<D4>();
                return provider.Invoke(d1, d2, d3, d4) ?? throw new NullReferenceException();
            });
        }

        /// <summary>
        /// Adds a scoped service as produced by the given provider on the <see cref="IServiceCollection"/>
        /// by first retrieving the dependencies from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="TService">the service type</typeparam>
        /// <typeparam name="D1">the type of the first dependency</typeparam>
        /// <typeparam name="D2">the type of the second dependency</typeparam>
        /// <typeparam name="D3">the type of the third dependency</typeparam>
        /// <typeparam name="D4">the type of the fourth dependency</typeparam>
        /// <typeparam name="D5">the type of the fifth dependency</typeparam>
        /// <param name="services">the services</param>
        /// <param name="provider">the service provider</param>
        /// <returns>this <see cref="IServiceCollection"/> for chaining</returns>
        /// <exception cref="InvalidOperationException">If services were not found.</exception>
        /// <exception cref="NullReferenceException">If the provider returned null.</exception>
        internal static IServiceCollection AddScopedProvider<TService, D1, D2, D3, D4, D5>(
            this IServiceCollection services, Func<D1, D2, D3, D4, D5, TService> provider) where TService : class
        {
            return services.AddScoped(sp =>
            {
                var d1 = sp.GetRequiredService<D1>();
                var d2 = sp.GetRequiredService<D2>();
                var d3 = sp.GetRequiredService<D3>();
                var d4 = sp.GetRequiredService<D4>();
                var d5 = sp.GetRequiredService<D5>();
                return provider.Invoke(d1, d2, d3, d4, d5) ?? throw new NullReferenceException();
            });
        }

        #endregion
    }
}
