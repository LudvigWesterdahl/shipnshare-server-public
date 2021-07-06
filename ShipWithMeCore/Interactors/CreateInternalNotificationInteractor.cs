using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.SharedKernel;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="ICreateInternalNotificationUseCase"/>.
    /// </summary>
    internal sealed class CreateInternalNotificationInteractor : ICreateInternalNotificationUseCase
    {
        private readonly IInternalNotificationRepository internalNotificationRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="internalNotificationRepository">internal notifications repository</param>
        internal CreateInternalNotificationInteractor(IInternalNotificationRepository internalNotificationRepository)
        {
            Validate.That(internalNotificationRepository, nameof(internalNotificationRepository)).IsNot(null);
            this.internalNotificationRepository = internalNotificationRepository;
        }

        /// <inheritdoc cref="ICreateInternalNotificationUseCase.Create(IDictionary{string, string})"/>
        public async Task<bool> Create(IDictionary<string, string> languageCodeMessages)
        {
            if (languageCodeMessages == null || languageCodeMessages.Count == 0)
            {
                return false;
            }

            await internalNotificationRepository.Save(languageCodeMessages);

            return true;
        }
    }
}
