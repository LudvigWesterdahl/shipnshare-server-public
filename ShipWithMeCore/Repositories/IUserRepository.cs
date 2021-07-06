using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetById(long userId);

        Task<UserEntity> GetByEmail(string email);

        Task<UserEntity> GetByUserName(string userName);

        Task<IEnumerable<UserEntity>> GetAll();
    }
}
