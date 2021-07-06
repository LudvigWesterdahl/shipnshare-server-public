using System;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.Entities
{
    /// <summary>
    /// A tag that can be attached to a <see cref="PostEntity"/>.
    /// </summary>
    public sealed class TagEntity
    {
        public string Id { get; }

        public string Name { get; }

        public TagEntity(string id, string name)
        {
            Id = id;

            Validate.That(name, nameof(name)).IsNot(null);
            Validate.That(name.Length, nameof(name.Length)).IsGreaterThan(0);

            Name = name;
        }
    }
}
