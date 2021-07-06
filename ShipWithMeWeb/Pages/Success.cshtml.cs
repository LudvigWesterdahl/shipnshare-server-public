using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShipWithMeWeb.Pages
{
    public class SuccessModel : PageModel
    {

        public string Message { get; set; }

        public SuccessModel()
        {

        }

        public void OnGet(string message)
        {
            if (message != null)
            {
                Message = message;
            }
        }
    }
}
