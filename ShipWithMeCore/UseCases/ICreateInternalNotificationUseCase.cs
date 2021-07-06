using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShipWithMeCore.UseCases
{
    public interface ICreateInternalNotificationUseCase
    {
        /// <summary>
        /// Creates an internal notification in multiple languages where the language code is the key
        /// and the message is the value.
        /// </summary>
        /// <param name="languageCodeMessages">the internal notifications</param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> Create(IDictionary<string, string> languageCodeMessages);
    }
}
