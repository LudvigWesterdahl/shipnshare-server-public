using System.Collections.Generic;
using System.Linq;
using ShipWithMeCore.Entities;

namespace ShipWithMeWeb.Responses
{
    public class DistanceToPostResponse
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

        public double MetersToPickupLocation { get; init; }

        public string ImagePath { get; init; }

        public string FaviconUri { get; init; }

        internal DistanceToPostResponse(PostEntity post, double metersToPickupLocation)
        {
            Id = post.Id;

            OwnerUserName = post.Owner.UserName;

            CreatedAt = post.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");

            CreatedAtTimeZone = post.CreatedAtTimeZone.Id;

            LatitudePickupLocation = post.PickupLocation.Item1;

            LongitudePickupLocation = post.PickupLocation.Item2;

            OfferValueTitle = post.OfferValueTitle;

            Description = post.Description;

            StoreOrProductUri = post.StoreOrProductUri.AbsoluteUri;

            ShippingCost = post.ShippingCost;

            Currency = post.Currency;

            Tags = post.Tags.Select(tag => tag.Name).ToList();

            MetersToPickupLocation = metersToPickupLocation;

            ImagePath = post.ImagePath;

            FaviconUri = post.FaviconUri.AbsoluteUri;
        }
    }
}
