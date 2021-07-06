using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeWeb.RequestInputs
{
    public sealed class ResetPasswordInfo
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string ResetPasswordKey { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
