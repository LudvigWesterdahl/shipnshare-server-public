using System.Threading.Tasks;

namespace ShipWithMeCore.ExternalServices
{
    public interface ILocationService
    {
        /// <summary>
        /// Returns the distance in meters between two coordinates or null if invalid coordinates.
        /// </summary>
        /// <param name="lat1">latitude of first point</param>
        /// <param name="lon1">longitude of first point</param>
        /// <param name="lat2">latitude of second point</param>
        /// <param name="lon2">longitude of second point</param>
        /// <returns>the distance in meters</returns>
        Task<double?> MetersBetween(double lat1, double lon1, double lat2, double lon2);
    }
}
