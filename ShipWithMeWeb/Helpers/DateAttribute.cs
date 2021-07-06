using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeWeb.Helpers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = false)]
    public sealed class DateAttribute : ValidationAttribute
    {
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

            if (!DateTimeUtils.TryParse((string)value, out var _, format: DateTimeUtils.DefaultFormat))
            {
                return new ValidationResult(
                    $"invalid format {value} requires {DateTimeUtils.DefaultFormat}",
                    new[] { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }
    }
}
