using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface IPostRepository
    {
        Task<PostEntity> Save(PostEntity.GeneralPostInfo generalPostInfo);

        Task<bool> Delete(PostEntity post);

        Task<PostEntity> GetById(string postId);

        Task<IEnumerable<PostEntity>> GetByUserId(long userId);

        Task<IEnumerable<PostEntity>> GetAll();

        Task<bool> SetOpen(string postId, bool open);
    }
}
