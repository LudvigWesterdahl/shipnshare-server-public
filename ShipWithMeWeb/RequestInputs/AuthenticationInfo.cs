using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class AuthenticationInfo
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
