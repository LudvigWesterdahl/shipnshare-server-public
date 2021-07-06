using System;
using System.Threading.Tasks;

namespace ShipWithMeCore.ExternalServices
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email to the given address.
        /// </summary>
        /// <param name="title">the title or subject</param>
        /// <param name="message">the message</param>
        /// <param name="toEmail">the recipient</param>
        /// <returns>true if successful, false otherwise</returns>
        Task<bool> Send(string title, string message, string toEmail);
    }
}
