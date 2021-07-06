using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface IInternalNotificationRepository
    {
        Task<IEnumerable<InternalNotificationEntity>> GetAll();

        Task<IEnumerable<InternalNotificationEntity>> GetAllCreatedOnOrAfter(DateTime dateTime);

        Task<InternalNotificationEntity> Save(IDictionary<string, string> languageCodeMessages);
    }
}
