using System;
namespace ShipWithMeWeb.Responses
{
    public sealed class ReportedPostResponse
    {
        public string PostId { get; set; }

        public int PostVersion { get; set; }

        public string UserEmail { get; set; }

        public string CreatedAt { get; set; }

        public string Message { get; set; }
    }
}
