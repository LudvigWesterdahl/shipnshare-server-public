using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure.Repositories
{
    internal sealed class PostRepository : IPostRepository
    {
        private readonly ILogger<IPostRepository> logger;
        private readonly MainDbContext mainDbContext;

        internal PostRepository(ILogger<IPostRepository> logger, MainDbContext mainDbContext)
        {
            Validate.That(logger, nameof(logger)).IsNot(null);
            this.logger = logger;
            Validate.That(mainDbContext, nameof(mainDbContext)).IsNot(null);
            this.mainDbContext = mainDbContext;
        }

        private IEnumerable<PostEntity> PostsWhere(Expression<Func<Post, bool>> predicate)
        {
            return mainDbContext.Posts
                .Where(predicate)
                .Include(p => p.Owner)
                .Include(p => p.Tags)
                .AsEnumerable()
                .Select(p => p.ToPostEntity());
        }

        private Task<IEnumerable<PostEntity>> PostsWhereAsync(Expression<Func<Post, bool>> predicate)
        {
            return Task.Run(() => {
                return PostsWhere(predicate);
            });
        }

        private PostEntity PostWhere(string postId)
        {
            return PostsWhere(p => p.Id == postId).First();
        }

        private Task<PostEntity> PostWhereAsync(string postId)
        {
            return Task.Run(() =>
            {
                return PostWhere(postId);
            });
        }

        public async Task<PostEntity> Save(PostEntity.GeneralPostInfo generalPostInfo)
        {
            logger.LogInformation("Post with {Owner.Id}:{Owner.Email} is being saved...",
                generalPostInfo.Owner.Id,
                generalPostInfo.Owner.Email);

            // https://stackoverflow.com/questions/25441027/how-do-i-stop-entity-framework-from-trying-to-save-insert-child-objects

            var post = new Post
            {
                Id = RepositoryUtils.NewGuidString(),
                Version = 1,
                OwnerId = generalPostInfo.Owner.Id,
                CreatedAt = generalPostInfo.CreatedAt,
                CreatedAtTimeZone = generalPostInfo.CreatedAtTimeZone,
                LatitudePickupLocation = generalPostInfo.PickupLocation.Item1,
                LongitudePickupLocation = generalPostInfo.PickupLocation.Item2,
                OfferValueTitle = generalPostInfo.OfferValueTitle,
                Description = generalPostInfo.Description,
                StoreOrProductUri = generalPostInfo.StoreOrProductUri,
                ShippingCost = generalPostInfo.ShippingCost,
                Currency = generalPostInfo.Currency,
                Tags = generalPostInfo.Tags.Select(t => new Tag
                {
                    Id = RepositoryUtils.NewGuidString(),
                    Name = t.Name
                }).ToList(),
                ImagePath = generalPostInfo.ImagePath,
                Open = true
            };

            await mainDbContext.Posts.AddAsync(post);
            await mainDbContext.SaveChangesAsync();

            logger.LogInformation("Post with ID {Id} was successfully saved", post.Id);

            return PostWhere(post.Id);
        }

        public Task<bool> Update(PostEntity post)
        {
            // Need to fix this so that posts can be closed.
            throw new System.NotImplementedException();
        }

        public Task<bool> Delete(PostEntity post)
        {
            throw new System.NotImplementedException();
        }

        public Task<PostEntity> GetById(string postId)
        {
            return PostWhereAsync(postId);
        }

        public Task<IEnumerable<PostEntity>> GetByUserId(long userId)
        {
            return PostsWhereAsync(p => p.OwnerId == userId);
        }

        public Task<IEnumerable<PostEntity>> GetAll()
        {
            return PostsWhereAsync(p => true);
        }

        public async Task<bool> SetOpen(string postId, bool open)
        {
            var postEntity = (await PostsWhereAsync(p => p.Id == postId)).FirstOrDefault();

            if (postEntity == null)
            {
                return false;
            }

            var post = mainDbContext.Posts
                .Where(p => p.Id == postId)
                .First();

            post.Open = open;

            await mainDbContext.SaveChangesAsync();

            return true;
        }
    }
}
