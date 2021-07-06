using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure.Repositories
{
    internal sealed class TagRepository : ITagRepository
    {
        private readonly ILogger<ITagRepository> logger;
        private readonly MainDbContext mainDbContext;

        internal TagRepository(ILogger<ITagRepository> logger, MainDbContext mainDbContext)
        {
            this.logger = logger;
            this.mainDbContext = mainDbContext;
        }

        public async Task<TagEntity> Save(string tagName)
        {
            logger.LogInformation("Saving tag with name {Name}...", tagName);
            var tag = new Tag
            {
                Id = RepositoryUtils.NewGuidString(),
                Name = tagName
            };

            await mainDbContext.Tags.AddAsync(tag);
            await mainDbContext.SaveChangesAsync();

            logger.LogInformation("Saved tag with name {Name}", tagName);

            return new TagEntity(tag.Id, tag.Name);
        }

        public Task<bool> Update(TagEntity tag)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(TagEntity tag)
        {
            throw new NotImplementedException();
        }

        public Task<TagEntity> GetById(string tagId)
        {
            throw new NotImplementedException();
        }

        public Task<TagEntity> GetByName(string tagName)
        {
            return Task.Run(() =>
            {
                logger.LogInformation("Getting tag with name {Name}...", tagName);

                var tag = mainDbContext.Tags.Where(t => t.Name == tagName).FirstOrDefault();

                if (tag == null)
                {
                    logger.LogInformation("Found no tag with name {Name}...", tagName);
                    return null;
                }

                logger.LogInformation("Returning tag with name {Name}...", tagName);
                return new TagEntity(tag.Id, tag.Name);
            });
        }

        public Task<IEnumerable<TagEntity>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
