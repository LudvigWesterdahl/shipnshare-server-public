using System;
namespace ShipWithMeWeb.RequestInputs
{
    public sealed class ReviewUserInfo
    {
        public string PostId { get; set; }

        public int Rating { get; set; }

        public string Message { get; set; }
    }
}
