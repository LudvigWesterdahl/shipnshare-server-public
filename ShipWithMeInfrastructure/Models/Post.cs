using System;
using System.Collections.Generic;
using System.Linq;
using ShipWithMeCore.Entities;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class Post
    {
        public string Id { get; set; }

        public int Version { get; set; }

        public long OwnerId { get; set; }

        public User Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public TimeZoneInfo CreatedAtTimeZone { get; set; }

        public double LatitudePickupLocation { get; set; }

        public double LongitudePickupLocation { get; set; }

        public string OfferValueTitle { get; set; }

        public string Description { get; set; }

        public Uri StoreOrProductUri { get; set; }

        public decimal ShippingCost { get; set; }

        public string Currency { get; set; }

        public bool Open { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        public string ImagePath { get; set; }

        public PostEntity ToPostEntity()
        {
            var generalPostInfo = new PostEntity.GeneralPostInfo()
                .SetOwner(new UserEntity(Owner.Id, Owner.Email, Owner.UserName))
                .SetCreatedAt(CreatedAt)
                .SetCreatedAtTimeZone(CreatedAtTimeZone)
                .SetPickupLocation(Tuple.Create(LatitudePickupLocation, LongitudePickupLocation))
                .SetOfferValueTitle(OfferValueTitle)
                .SetDescription(Description)
                .SetStoreOrProductUri(StoreOrProductUri)
                .SetShippingCost(ShippingCost)
                .SetCurrency(Currency)
                .SetOpen(Open)
                .WithTags(Tags.Select(t => new TagEntity(t.Id, t.Name)))
                .SetImagePath(ImagePath);

            return PostEntity.Create(Id, generalPostInfo);
        }
    }
}
