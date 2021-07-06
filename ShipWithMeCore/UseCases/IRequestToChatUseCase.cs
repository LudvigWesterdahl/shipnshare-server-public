using System;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IRequestToChatUseCase
    {
        /// <summary>
        /// A chat is created in regards to the provided post and the owner of that post is invited to participate.
        /// </summary>
        /// <param name="postId">the post id</param>
        /// <param name="userId">the user id</param>
        /// <returns>the created chat request, null if chat request was not created</returns>
        Task<ChatRequestEntity> Request(string postId, long userId);
    }
}
