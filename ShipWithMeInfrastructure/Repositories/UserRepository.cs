using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly ILogger<IUserRepository> logger;

        private readonly MainDbContext mainDbContext;

        internal UserRepository(ILogger<IUserRepository> logger, MainDbContext mainDbContext)
        {
            this.logger = logger;
            this.mainDbContext = mainDbContext;
        }

        private UserEntity UserWhere(Expression<Func<User, bool>> predicate)
        {
            return mainDbContext.Users
                .Where(predicate)
                .AsEnumerable()
                .Select(u => u.ToUserEntity())
                .FirstOrDefault();
        }

        private Task<UserEntity> UserWhereAsync(Expression<Func<User, bool>> predicate)
        {
            return Task.Run(() =>
            {
                return UserWhere(predicate);
            });
        }

        public Task<IEnumerable<UserEntity>> GetAll()
        {
            return Task.Run(() =>
            {
                return mainDbContext.Users
                    .AsEnumerable()
                    .Select(u => u.ToUserEntity());
            });
        }

        public Task<UserEntity> GetByEmail(string email)
        {
            return UserWhereAsync(u => u.Email == email);
        }

        public Task<UserEntity> GetById(long userId)
        {
            return UserWhereAsync(u => u.Id == userId);
        }

        public Task<UserEntity> GetByUserName(string userName)
        {
            return UserWhereAsync(u => u.UserName == userName);
        }
    }
}
