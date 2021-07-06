using Microsoft.AspNetCore.Hosting;

namespace ShipWithMeWeb.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IWebHostEnvironment"/>.
    /// </summary>
    public static class IWebhostEnvironmentExtensions
    {
        /// <summary>
        /// Returns true if the <see cref="IHostingEnvironment.EnvironmentName"/> is either production or develop.
        /// </summary>
        /// <param name="env">the environment</param>
        /// <returns>true if the EnvironmentName is either production or develop, false otherwise</returns>
        public static bool HasValidEnvironmentName(this IWebHostEnvironment env)
        {
            return "production".Equals(env.EnvironmentName) || "develop".Equals(env.EnvironmentName);
        }

        /// <summary>
        /// Returns true if the <see cref="IHostingEnvironment.EnvironmentName"/> equals production.
        /// </summary>
        /// <param name="env">the environment</param>
        /// <returns>true if EnvironmentName is production, false otherwise</returns>
        public static bool IsProduction(this IWebHostEnvironment env)
        {
            if (!env.HasValidEnvironmentName()) {
                // TODO: Log this
                return false;
            }

            return env.EnvironmentName.Equals("production");
        }

        /// <summary>
        /// Returns true if the <see cref="IHostingEnvironment.EnvironmentName"/> equals develop.
        /// </summary>
        /// <param name="env">the environment</param>
        /// <returns>true if EnvironmentName is develop, false otherwise</returns>
        public static bool IsDevelop(this IWebHostEnvironment env)
        {
            if (!env.HasValidEnvironmentName()) {
                // TODO: Log this
                return false;
            }

            return env.EnvironmentName.Equals("develop");
        }
    }
}
