using System;
using System.Collections.Generic;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class Tag
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}
