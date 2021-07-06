using System;
namespace ShipWithMeInfrastructure.Models
{
    public sealed class RefreshToken
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public long UserId { get; set; }

        public User User { get; set; }
    }
}
