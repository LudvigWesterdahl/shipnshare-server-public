using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShipWithMeCore.Entities;

namespace ShipWithMeCore.Repositories
{
    public interface ITagRepository
    {
        Task<TagEntity> Save(string tagName);

        Task<bool> Update(TagEntity tag);

        Task<bool> Delete(TagEntity tag);

        Task<TagEntity> GetById(string tagId);

        Task<TagEntity> GetByName(string tagName);

        Task<IEnumerable<TagEntity>> GetAll();
    }
}
