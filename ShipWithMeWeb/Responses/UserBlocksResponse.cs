using System;
using System.Collections;
using System.Collections.Generic;

namespace ShipWithMeWeb.Responses
{
    public class UserBlocksResponse
    {
        public bool Blocked { get; set; }

        public string CurrentDateTime { get; set; }

        public IEnumerable<UserBlockResponse> UserBlocks { get; set; }
    }
}
