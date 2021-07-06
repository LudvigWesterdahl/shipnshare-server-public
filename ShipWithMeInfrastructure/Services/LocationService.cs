using System;
using System.Threading.Tasks;
using ShipWithMeCore.ExternalServices;

namespace ShipWithMeInfrastructure.Services
{
    public sealed class LocationService : ILocationService
    {
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <inheritdoc cref="ILocationService.MetersBetween(double, double, double, double)"/>
        public Task<double?> MetersBetween(double lat1, double lon1, double lat2, double lon2)
        {
            return Task.Run(() =>
            {
                if (lat1.CompareTo(-90) < 0
                    || lat1.CompareTo(90) > 0
                    || lon1.CompareTo(-180) < 0
                    || lon1.CompareTo(180) > 0
                    || lat2.CompareTo(-90) < 0
                    || lat2.CompareTo(90) > 0
                    || lon2.CompareTo(-180) < 0
                    || lon2.CompareTo(180) > 0)
                {
                    return null;
                }

                double radiansLat = ToRadians(lat1 - lat2);
                double radiansLon = ToRadians(lon1 - lon2);

                double latSin = Math.Sin(radiansLat / 2) * Math.Sin(radiansLat / 2);
                double lonSin = Math.Sin(radiansLon / 2) * Math.Sin(radiansLon / 2);
                double latCos = Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2));

                double a = latSin + latCos * lonSin;
                double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

                return (double?)c * 6371000;
            });
        }
    }
}
