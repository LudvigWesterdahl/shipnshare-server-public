using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;
namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IGetUserUseCase"/>.
    /// </summary>
    internal sealed class GetUserInteractor : IGetUserUseCase
    {
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepository">user repository</param>
        internal GetUserInteractor(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <inheritdoc cref="IGetUserUseCase.GetById(long)"/>
        public Task<UserEntity> GetById(long userId)
        {
            return userRepository.GetById(userId);
        }

        /// <inheritdoc cref="IGetUserUseCase.GetByUserName(string)"/>
        public Task<UserEntity> GetByUserName(string userName)
        {
            Validate.That(userName, nameof(userName)).IsNot(null);
            return userRepository.GetByUserName(userName);
        }

        /// <inheritdoc cref="IGetUserUseCase.GetByEmail(string)"/>
        public Task<UserEntity> GetByEmail(string email)
        {
            Validate.That(email, nameof(email)).IsNot(null);
            return userRepository.GetByEmail(email);
        }
    }
}
