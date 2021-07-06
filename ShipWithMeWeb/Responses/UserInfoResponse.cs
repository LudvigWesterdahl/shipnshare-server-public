using System;
namespace ShipWithMeWeb.Responses
{
    public sealed class UserInfoResponse
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public bool HasRequestedUserInfo { get; set; }

        public long RequestedUserId { get; set; }

        public string RequestedUserName { get; set; }
    }
}
