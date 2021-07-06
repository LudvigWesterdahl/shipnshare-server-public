using System;
namespace ShipWithMeWeb.Responses
{
    public sealed class UserBlockResponse
    {
        public string Id { get; set; }

        public int Version { get; set; }

        public string UserEmail { get; set; }

        public string Reason { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public bool Active { get; set; }
    }
}
