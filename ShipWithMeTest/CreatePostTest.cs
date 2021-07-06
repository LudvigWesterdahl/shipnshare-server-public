using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;
using ShipWithMeInfrastructure;
using ShipWithMeInfrastructure.Models;
using ShipWithMeInfrastructure.Repositories;
using Xunit;

namespace ShipWithMeTest
{
    public sealed class CreatePostTest
    {
        private MainDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<MainDbContext>()
                      .UseInMemoryDatabase(databaseName: "TestDB")
                      .Options;

            return new MainDbContext(options);
        }

        private IPostRepository CreatePostRepository(MainDbContext mainDbContext)
        {
            var logger = new Mock<ILogger<IPostRepository>>();
            return RepositoryFactory.NewPostRepository(logger.Object, mainDbContext);
        }

        private UserEntity CreateUser(MainDbContext mainDbContext, string email, string userName)
        {
            var user = new User
            {
                Id = new Random().Next(int.MaxValue),
                Email = email,
                UserName = userName
            };

            mainDbContext.Users.Add(user);
            mainDbContext.SaveChanges();

            return user.ToUserEntity();
        }

        [Fact]
        public async Task Create_NewPost_Created()
        {
            var context = CreateContext();
            var repository = CreatePostRepository(context);
            var user = CreateUser(context, "admin@admin.com", "admin");
            var createPostUseCase = UseCaseFactory.NewCreatePostUseCase(repository);

            var generalPostInfo = new PostEntity.GeneralPostInfo()
                .SetCreatedAt(DateTime.UtcNow)
                .SetCreatedAtTimeZone(TimeZoneInfo.Local)
                .SetCurrency("SEK")
                .SetOpen(true)
                .SetOfferValueTitle("This title")
                .SetStoreOrProductUri(new Uri("https://amazon.se"))
                .SetShippingCost(10)
                .SetTags(new List<TagEntity>())
                .SetDescription("This description")
                .SetPickupLocation(Tuple.Create(10D, 10D))
                .SetOwner(user);

            var post = await createPostUseCase.Create(generalPostInfo);

            context.Dispose();

            Assert.NotNull(post);
            Assert.NotNull(post.Id);
            Assert.Equal("SEK", post.Currency);
        }
    }
}
