using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.Helpers
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class LatitudeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult(
                    "null not allowed",
                    new[] { validationContext.DisplayName });
            }

            if (!(value is double || value is decimal))
            {
                return new ValidationResult(
                    $"invalid type {value.GetType()} requires double or decimal",
                    new[] { validationContext.DisplayName });
            }

            var latitude = Convert.ToDecimal(value);

            if (latitude < -90 || latitude > 90)
            {
                return new ValidationResult(
                    "a latitude has to be in the range of -90 and 90",
                    new[] { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }
    }
}
