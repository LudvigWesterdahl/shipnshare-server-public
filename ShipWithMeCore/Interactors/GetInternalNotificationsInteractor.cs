using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="IGetInternalNotificationsUseCase"/>.
    /// </summary>
    internal class GetInternalNotificationsInteractor : IGetInternalNotificationsUseCase
    {
        private readonly IInternalNotificationRepository internalNotificationRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="internalNotificationRepository">internal notifications repository</param>
        internal GetInternalNotificationsInteractor(IInternalNotificationRepository internalNotificationRepository)
        {
            Validate.That(internalNotificationRepository, nameof(internalNotificationRepository)).IsNot(null);
            this.internalNotificationRepository = internalNotificationRepository;
        }

        /// <inheritdoc cref="IGetInternalNotificationsUseCase.GetForDateTime(DateTime)"/>
        public async Task<IEnumerable<InternalNotificationEntity>> GetForDateTime(DateTime dateTime)
        {
            var internalNotifications = await internalNotificationRepository
                .GetAllCreatedOnOrAfter(dateTime.AddHours(-1));

            return internalNotifications;
        }
    }
}
