using System;
using System.ComponentModel.DataAnnotations;

namespace ShipWithMeInfrastructure.Models
{
    public sealed class UserBlock
    {
        public string Id { get; set; }

        public int Version { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
