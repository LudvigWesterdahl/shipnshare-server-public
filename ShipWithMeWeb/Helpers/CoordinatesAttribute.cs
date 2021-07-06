using System;
using System.ComponentModel.DataAnnotations;
using ShipWithMeWeb.RequestInputs;

namespace ShipWithMeWeb.Helpers
{
    /// <summary>
    /// How to test: https://stackoverflow.com/questions/34765859/testing-validationattribute-that-overrides-isvalid
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class CoordinatesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.ObjectInstance is Coordinates))
            {
                return new ValidationResult(
                    "invalid type " + validationContext.ObjectInstance.GetType().ToString(),
                    new[] { nameof(Coordinates) });
            }

            var coordinates = (Coordinates)validationContext.ObjectInstance;

            if (coordinates.Latitude < -90 || coordinates.Latitude > 90)
            {
                return new ValidationResult(
                    "the latitude has to be in the range of -90 and 90",
                    new[] { nameof(Coordinates) });
            }

            if (coordinates.Longitude < -180 || coordinates.Longitude > 180)
            {
                return new ValidationResult(
                    "the longitude has to be in the range of -180 and 180",
                    new[] { nameof(Coordinates) });
            }

            return ValidationResult.Success;
        }
    }
}
