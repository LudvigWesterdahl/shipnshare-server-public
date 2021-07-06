using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.Helpers
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class LongitudeAttribute : ValidationAttribute
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

            var longitude = Convert.ToDecimal(value);

            if (longitude < -180 || longitude > 180)
            {
                return new ValidationResult(
                    "a longitude has to be in the range of -180 and 180",
                    new[] { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }
    }
}
