using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.DataTransferObjects;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface IGetPostsUseCase
    {
        Task<IEnumerable<DistanceToPostDto>> GetAll(double latitude, double longitude);

        Task<IEnumerable<DistanceToPostDto>> GetAll(double latitude, double longitude, double maxDistance);

        Task<IEnumerable<PostEntity>> GetByUserId(long userId, bool onlyOpen);

        Task<PostEntity> GetById(string postId);
    }
}
