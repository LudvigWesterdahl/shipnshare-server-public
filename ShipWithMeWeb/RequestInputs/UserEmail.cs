using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class UserEmail
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
