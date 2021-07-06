using System.Collections.Generic;
using System.Linq;
using ShipWithMeCore.Entities;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeWeb.Responses
{
    public sealed class PostResponse
    {
        public string Id { get; init; }

        public string OwnerUserName { get; init; }

        public string CreatedAt { get; init; }

        public string CreatedAtTimeZone { get; init; }

        public double LatitudePickupLocation { get; init; }

        public double LongitudePickupLocation { get; init; }

        public string OfferValueTitle { get; init; }

        public string Description { get; init; }

        public string StoreOrProductUri { get; init; }

        public decimal ShippingCost { get; init; }

        public string Currency { get; init; }

        public IList<string> Tags { get; init; }

        public string FaviconUri { get; init; }

        public string ImagePath { get; init; }

        internal PostResponse(PostEntity post)
        {
            Id = post.Id;

            OwnerUserName = post.Owner.UserName;

            CreatedAt = DateTimeUtils.ToString(post.CreatedAt);

            CreatedAtTimeZone = post.CreatedAtTimeZone.Id;

            LatitudePickupLocation = post.PickupLocation.Item1;

            LongitudePickupLocation = post.PickupLocation.Item2;

            OfferValueTitle = post.OfferValueTitle;

            Description = post.Description;

            StoreOrProductUri = post.StoreOrProductUri.AbsoluteUri;

            ShippingCost = post.ShippingCost;

            Currency = post.Currency;

            FaviconUri = post.FaviconUri.AbsoluteUri;

            Tags = post.Tags.Select(tag => tag.Name).ToList();

            ImagePath = post.ImagePath;
        }
    }
}
