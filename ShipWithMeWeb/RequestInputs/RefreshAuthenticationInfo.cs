using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class RefreshAuthenticationInfo
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
