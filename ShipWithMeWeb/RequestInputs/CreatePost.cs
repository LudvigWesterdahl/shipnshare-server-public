using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using ShipWithMeCore.Entities;
using ShipWithMeCore.SharedKernel;
using ShipWithMeWeb.Helpers;
using static ShipWithMeCore.Entities.PostEntity;

namespace ShipWithMeWeb.RequestInputs
{
    public class CreatePost
    {
        [Date]
        public string CreatedAt { get; init; }

        [TimeZone]
        public string CreatedAtTimeZone { get; init; }

        [Required]
        [Latitude]
        public double? LatitudePickupLocation { get; init; }

        [Required]
        [Longitude]
        public double? LongitudePickupLocation { get; init; }

        [Required]
        public string OfferValueTitle { get; init; }

        [Required]
        public string Description { get; init; }

        [Required]
        public string StoreOrProductUri { get; init; }

        [Required]
        public decimal? ShippingCost { get; init; }

        [Required]
        public string Currency { get; init; }

        public string ImageType { get; init; }

        public string Image { get; init; }

        public IEnumerable<string> Tags { get; init; }

        public GeneralPostInfo ToGeneralPostInfo(UserEntity owner)
        {
            return new GeneralPostInfo()
                .SetOwner(owner)
                .SetCreatedAt(DateTimeUtils.Parse(CreatedAt))
                .SetCreatedAtTimeZone(TimeZoneInfo.FindSystemTimeZoneById(CreatedAtTimeZone))
                .SetPickupLocation(Tuple.Create(LatitudePickupLocation.Value, LongitudePickupLocation.Value))
                .SetOfferValueTitle(OfferValueTitle)
                .SetDescription(Description)
                .SetStoreOrProductUri(new Uri(StoreOrProductUri))
                .SetShippingCost(ShippingCost.Value)
                .SetCurrency(Currency)
                .WithTags(Tags.Select(tag => new TagEntity("", tag)));
        }

        public GeneralPostInfo ToGeneralPostInfo(UserEntity owner, string imagePath)
        {
            return new GeneralPostInfo()
                .SetOwner(owner)
                .SetCreatedAt(DateTimeUtils.Parse(CreatedAt))
                .SetCreatedAtTimeZone(TimeZoneInfo.FindSystemTimeZoneById(CreatedAtTimeZone))
                .SetPickupLocation(Tuple.Create(LatitudePickupLocation.Value, LongitudePickupLocation.Value))
                .SetOfferValueTitle(OfferValueTitle)
                .SetDescription(Description)
                .SetStoreOrProductUri(new Uri(StoreOrProductUri))
                .SetShippingCost(ShippingCost.Value)
                .SetCurrency(Currency)
                .WithTags(Tags.Select(tag => new TagEntity("", tag)))
                .SetImagePath(imagePath);
        }
    }
}
