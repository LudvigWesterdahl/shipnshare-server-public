using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.DataTransferObjects;
using ShipWithMeCore.Entities;
using ShipWithMeCore.ExternalServices;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IGetPostsUseCase"/>.
    /// </summary>
    internal sealed class GetPostsInteractor : IGetPostsUseCase
    {
        private readonly IPostRepository postRepository;

        private readonly ILocationService locationService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="postRepository">post repository</param>
        /// <param name="locationService">location service</param>
        internal GetPostsInteractor(IPostRepository postRepository, ILocationService locationService)
        {
            Validate.That(postRepository, nameof(postRepository)).IsNot(null);
            this.postRepository = postRepository;

            Validate.That(locationService, nameof(locationService)).IsNot(null);
            this.locationService = locationService;
        }

        /// <inheritdoc cref="IGetPostsUseCase.GetAll(double, double)"/>
        public async Task<IEnumerable<DistanceToPostDto>> GetAll(double latitude, double longitude)
        {
            return await GetAll(latitude, longitude, 100000D);
        }

        /// <inheritdoc cref="IGetPostsUseCase.GetAll(double, double, double)"/>
        public async Task<IEnumerable<DistanceToPostDto>> GetAll(double latitude, double longitude, double maxDistance)
        {
            var posts = await postRepository.GetAll();

            var distanceToPosts = new List<DistanceToPostDto>();
            foreach (var post in posts)
            {
                var distance = await locationService.MetersBetween(
                    post.PickupLocation.Item1, post.PickupLocation.Item2, latitude, longitude);

                if (distance == null)
                {
                    throw new ArgumentException($"Bad coordinates (lat, long)({latitude}, {longitude})");
                }
                else if (distance <= maxDistance && post.Open)
                {
                    distanceToPosts.Add(new DistanceToPostDto(post, distance.Value));
                }
            }

            return distanceToPosts;
        }

        /// <inheritdoc cref="IGetPostsUseCase.GetById(string)"/>
        public async Task<PostEntity> GetById(string postId)
        {
            var post = await postRepository.GetById(postId);

            return post;
        }

        /// <inheritdoc cref="IGetPostsUseCase.GetByUserId(long, bool)"/>
        public async Task<IEnumerable<PostEntity>> GetByUserId(long userId, bool onlyOpen)
        {
            var posts = await postRepository.GetByUserId(userId);

            if (onlyOpen)
            {
                return posts.Where(p => p.Open)
                    .AsEnumerable();
            }

            return posts;
        }
    }
}
