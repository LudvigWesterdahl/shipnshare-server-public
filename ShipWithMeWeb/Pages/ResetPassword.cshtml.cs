using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShipWithMeCore.UseCases;
using ShipWithMeWeb.Authentication;

namespace ShipWithMeWeb.Pages
{
    public class ResetPasswordModel : PageModel
    {
        public long GetUserId { get; set; }

        public string GetToken { get; set; }

        [BindProperty]
        public long UserId { get; set; }

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        private readonly AuthenticationHelper authenticationHelper;

        public ResetPasswordModel(AuthenticationHelper authenticationHelper)
        {
            this.authenticationHelper = authenticationHelper;
        }

        // https://stackoverflow.com/questions/52693364/asp-net-core-2-1-razor-page-return-page-with-model
        public void OnGet([FromQuery(Name = "userid")] long userId, [FromQuery(Name = "token")] string token)
        {
            if (userId == 0 || token == null)
            {
                throw new ArgumentException("Invalid token");
            }

            GetUserId = userId;
            GetToken = token;
        }

        public IActionResult OnPost()
        {
            var success = authenticationHelper.ResetPasswordWithToken(UserId, Token, NewPassword);

            if (!success)
            {
                throw new ArgumentException("Password reset failed");
            }

            return RedirectToPage("Success", new
            {
                Message = "Password was successfully reset!"
            });
        }
    }
}
