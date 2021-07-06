using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeInfrastructure.Models;

namespace ShipWithMeInfrastructure.Repositories
{
    internal sealed class InternalNotificationRepository : IInternalNotificationRepository
    {
        private readonly ILogger<IInternalNotificationRepository> logger;
        private readonly MainDbContext mainDbContext;

        internal InternalNotificationRepository(ILogger<IInternalNotificationRepository> logger, MainDbContext mainDbContext)
        {
            Validate.That(logger, nameof(logger)).IsNot(null);
            this.logger = logger;
            Validate.That(mainDbContext, nameof(mainDbContext)).IsNot(null);
            this.mainDbContext = mainDbContext;
        }

        /// <inheritdoc cref="IInternalNotificationRepository.GetAll"/>
        public Task<IEnumerable<InternalNotificationEntity>> GetAll()
        {
            return Task.Run(() =>
            {
                var entities = mainDbContext.InternalNotifications
                    .Include(i => i.InternalNotificationMessages)
                    .AsEnumerable()
                    .Select(i => i.ToInternalNotificationEntity());

                return entities;
            });
        }

        /// <inheritdoc cref="IInternalNotificationRepository.GetAllCreatedOnOrAfter(DateTime)"/>
        public Task<IEnumerable<InternalNotificationEntity>> GetAllCreatedOnOrAfter(DateTime dateTime)
        {
            return Task.Run(() =>
            {
                var entities = mainDbContext.InternalNotifications
                    .Include(i => i.InternalNotificationMessages)
                    .Where(i => i.CreatedAt >= dateTime)
                    .AsEnumerable()
                    .Select(i => i.ToInternalNotificationEntity())
                    .AsEnumerable();

                return entities;
            });
        }

        /// <inheritdoc cref="IInternalNotificationRepository.Save(IDictionary{string, string})"/>
        public async Task<InternalNotificationEntity> Save(IDictionary<string, string> languageCodeMessages)
        {

            var id = await RepositoryUtils.NewGuidString(guid => mainDbContext.InternalNotifications.FindAsync(guid));

            var internalNotificationMessages = languageCodeMessages.Select(kv => new InternalNotificationMessage
            {
                InternalNotificationId = id,
                LanguageCode = kv.Key,
                Message = kv.Value
            }).ToList();

            var internalNotification = new InternalNotification
            {
                Id = id,
                CreatedAt = DateTime.UtcNow,
                InternalNotificationMessages = internalNotificationMessages
            };

            await mainDbContext.InternalNotifications.AddAsync(internalNotification);
            await mainDbContext.SaveChangesAsync();

            return internalNotification.ToInternalNotificationEntity();
        }
    }
}
