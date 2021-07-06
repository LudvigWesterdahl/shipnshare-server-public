using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ShipWithMeCore.SharedKernel;

namespace ShipWithMeWeb.Helpers
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public sealed class QueryDateAttribute : ValidationAttribute
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

            if (!DateTimeUtils.TryParse((string)value, out var _, format: DateTimeUtils.QueryFormat))
            {
                return new ValidationResult(
                    $"invalid format {value} requires {DateTimeUtils.QueryFormat}",
                    new[] { validationContext.DisplayName });
            }

            return ValidationResult.Success;
        }
    }
}
