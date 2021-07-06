using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShipWithMeWeb.Helpers;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class Coordinates
    {
        //[Required]
        [Latitude]
        public double? Latitude { get; set; }

        //[Required]
        [Longitude]
        public double? Longitude { get; set; }
    }
}
