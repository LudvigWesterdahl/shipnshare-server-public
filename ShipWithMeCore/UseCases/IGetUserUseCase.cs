using System;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IGetUserUseCase
    {
        Task<UserEntity> GetById(long userId);

        Task<UserEntity> GetByUserName(string userName);

        Task<UserEntity> GetByEmail(string email);
    }
}
