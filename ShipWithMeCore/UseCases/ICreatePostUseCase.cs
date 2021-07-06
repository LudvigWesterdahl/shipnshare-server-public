using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.UseCases
{
    public interface ICreatePostUseCase
    {
        Task<PostEntity> Create(PostEntity.GeneralPostInfo generalPostInfo);
    }
}
