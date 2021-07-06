using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace ShipWithMeCore.SharedKernel
{
    /// <summary>
    /// Utility class to standardize datetimes across the project.
    /// </summary>
    public static class DateTimeUtils
    {
        public const string DefaultFormat = "yyyy-MM-dd HH:mm:ss";
        public const string QueryFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// Returns the date time formatted using the given format.
        /// </summary>
        /// <param name="dateTime">the datetime</param>
        /// <param name="format">the format to use, defaults to <see cref="DefaultFormat"/></param>
        /// <returns>string representation</returns>
        public static string ToString(DateTime dateTime, string format = DefaultFormat)
        {
            return dateTime.ToString(format);
        }

        /// <summary>
        /// Tries to parse the string with the expected format as returned
        /// from <see cref="ToString(DateTime, string)"/>.
        /// </summary>
        /// <param name="dateTimeString">the datetime as string</param>
        /// <param name="dateTime">returned datetime</param>
        /// <param name="format">the format to use, defaults to <see cref="DefaultFormat"/></param>
        /// <returns>true if successful, false otherwise</returns>
        public static bool TryParse(string dateTimeString, out DateTime dateTime, string format = DefaultFormat)
        {
            if (DateTime.TryParseExact(
                dateTimeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var newDateTime))
            {
                dateTime = DateTime.SpecifyKind(newDateTime, DateTimeKind.Utc);

                return true;
            }

            dateTime = default;
            return false;
        }

        /// <summary>
        /// Parses the string with the expected format as returned from <see cref="ToString(DateTime, string)"/>.
        /// </summary>
        /// <param name="dateTimeString">the datetime as string</param>
        /// <param name="format">the format to use, defaults to <see cref="DefaultFormat"/></param>
        /// <returns>the datetime</returns>
        /// <exception cref="ArgumentException">if dateTimeString has invalid format</exception>
        public static DateTime Parse(string dateTimeString, string format = DefaultFormat)
        {
            var validDateTimeString = TryParse(dateTimeString, out var dateTime, format: format);

            if (!validDateTimeString)
            {
                throw new ArgumentException($"dateTimeString({dateTimeString}) requires format {format}");
            }

            return dateTime;
        }
    }
}
