using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShipWithMeCore.DataTransferObjects;
using ShipWithMeCore.Entities;
using ShipWithMeCore.Repositories;
using ShipWithMeCore.UseCases;

namespace ShipWithMeCore.Interactors
{
    /// <summary>
    /// Interactor for <see cref="ICreatePostUseCase"/>.
    /// </summary>
    internal sealed class CreatePostInteractor : ICreatePostUseCase
    {
        private readonly IPostRepository postRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="postRepository">post repository</param>
        internal CreatePostInteractor(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        /// <inheritdoc cref="ICreatePostUseCase.Create(PostEntity.GeneralPostInfo)"/>
        public async Task<PostEntity> Create(PostEntity.GeneralPostInfo generalPostInfo)
        {
            /*
            var tags = new List<TagEntity>();

            foreach (var tagName in generalPostInfo.Tags.Select(t => t.Name))
            {
                var tag = await tagRepository.GetByName(tagName);

                if (tag == null)
                {
                    tag = await tagRepository.Save(tagName);
                }

                tags.Add(tag);
            }

            generalPostInfo.SetTags(tags);
            */

            var post = await postRepository.Save(generalPostInfo);

            return post;
        }
    }
}
