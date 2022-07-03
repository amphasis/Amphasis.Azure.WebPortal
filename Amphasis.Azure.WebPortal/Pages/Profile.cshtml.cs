using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphasis.Azure.WebPortal.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public ProfileModel()
        {

        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/");
            return SignOut();
        }
    }
}
