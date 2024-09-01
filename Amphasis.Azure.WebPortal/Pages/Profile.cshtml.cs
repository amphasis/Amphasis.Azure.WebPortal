using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amphasis.Azure.WebPortal.Pages;

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
		return SignOut(new AuthenticationProperties {RedirectUri = "/"});
	}
}