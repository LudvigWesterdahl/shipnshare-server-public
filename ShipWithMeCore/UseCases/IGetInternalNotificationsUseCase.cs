using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IGetInternalNotificationsUseCase
    {
        /// <summary>
        /// Returns all <see cref="InternalNotificationEntity"/> that are active given the provided datetime.
        /// </summary>
        /// <param name="dateTime">the datetime</param>
        /// <returns>the notifications</returns>
        Task<IEnumerable<InternalNotificationEntity>> GetForDateTime(DateTime dateTime);
    }
}
