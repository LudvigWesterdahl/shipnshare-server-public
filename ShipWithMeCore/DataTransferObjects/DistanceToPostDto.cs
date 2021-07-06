using ShipWithMeCore.Entities;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeCore.DataTransferObjects
{
    public sealed class DistanceToPostDto
    {
        public PostEntity Post { get; }

        public double MetersToPickupLocation { get; }

        internal DistanceToPostDto(PostEntity post, double metersToPickupLocation)
        {
            Validate.That(post, nameof(post)).IsNot(null);
            Validate.That(metersToPickupLocation, nameof(metersToPickupLocation)).IsNotLessThan(0D);
            Post = post;
            MetersToPickupLocation = metersToPickupLocation;
        }
    }
}
