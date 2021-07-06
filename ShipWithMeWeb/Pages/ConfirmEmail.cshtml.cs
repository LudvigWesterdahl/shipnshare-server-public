using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShipWithMeWeb.Authentication;

namespace ShipWithMeWeb.Pages
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly AuthenticationHelper authenticationHelper;

        public ConfirmEmailModel(AuthenticationHelper authenticationHelper)
        {
            this.authenticationHelper = authenticationHelper;
        }

        public IActionResult OnGet([FromQuery(Name = "userid")] long userId, [FromQuery(Name = "token")] string token)
        {
            if (userId == 0 || token == null)
            {
                throw new ArgumentException("Invalid token");
            }

            var success = authenticationHelper.ConfirmEmail(userId, token);

            if (!success)
            {
                throw new ArgumentException("Invalid token");
            }

            return RedirectToPage("Success", new
            {
                Message = "Email was successfully confirmed!"
            });
        }
    }
}
