using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ShipWithMeWeb.Authentication
{
    public sealed class UserNotBlockedHandler : IAuthorizationHandler
    {
        private readonly AuthenticationHelper authenticationHelper;

        public UserNotBlockedHandler(AuthenticationHelper authenticationHelper)
        {
            this.authenticationHelper = authenticationHelper;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var userEmail = AuthenticationHelper.GetUserEmail(context.User);

            if (userEmail == null)
            {
                context.Succeed(null);
                return;
            }

            if (await authenticationHelper.IsBlocked(userEmail))
            {
                context.Fail();
            }
            else
            {
                context.Succeed(null);
            }
        }
    }
}
