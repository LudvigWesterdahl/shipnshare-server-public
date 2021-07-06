using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShipWithMeWeb.Helpers
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class TimeZoneAttribute : ValidationAttribute
    {
        private readonly IList<string> TimeZoneIds = TimeZoneInfo.GetSystemTimeZones().Select(tz => tz.Id).ToList();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(
                    "null not allowed",
                    new[] { validationContext.DisplayName });
            }

            if (!(value is string))
            {
                return new ValidationResult(
                     $"invalid type {value.GetType()} requires string",
                    new[] { validationContext.DisplayName });
            }

            var timeZoneString = (string)value;

            if (!TimeZoneIds.Contains(timeZoneString))
            {
                return new ValidationResult(
                    "invalid time zone " + timeZoneString,
                    new[] { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }
    }
}
