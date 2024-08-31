using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Amphasis.Azure.WebPortal.Pages;

public class SignInModel : PageModel
{
	public class AuthenticationSchemeInfo
	{
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string IconUrl { get; set; }
	}

	private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
	private readonly IFileProvider _fileProvider;

	public SignInModel(
		IAuthenticationSchemeProvider authenticationSchemeProvider,
		IOptions<StaticFileOptions> staticFileOptions,
		IWebHostEnvironment webHostEnvironment)
	{
		_authenticationSchemeProvider = authenticationSchemeProvider;
		_fileProvider = staticFileOptions.Value.FileProvider ?? webHostEnvironment.WebRootFileProvider;
	}

	public IEnumerable<AuthenticationSchemeInfo> AuthenticationSchemes { get; set; }

	[FromQuery]
	[FromForm]
	public string ReturnUrl { get; set; }

	[FromForm]
	public string Provider { get; set; }

	public async Task<ActionResult> OnGet()
	{
		if (User.Identity?.IsAuthenticated == true)
			return Redirect(ReturnUrl ?? Url.Page("Index")!);

		var schemesEnumerable = await _authenticationSchemeProvider.GetAllSchemesAsync();

		AuthenticationSchemes = schemesEnumerable
			.Where(scheme => !string.IsNullOrEmpty(scheme.DisplayName))
			.Select(scheme => new AuthenticationSchemeInfo
			{
				Name = scheme.Name,
				DisplayName = scheme.DisplayName,
				IconUrl = GetProviderIconUrl(scheme.Name)
			});

		return Page();
	}

	public async Task<ActionResult> OnPost()
	{
		var schemesEnumerable = await _authenticationSchemeProvider.GetAllSchemesAsync();
		var comparer = StringComparer.InvariantCultureIgnoreCase;
		var isSupportedProvider = schemesEnumerable.Select(scheme => scheme.Name).Contains(Provider, comparer);

		if (!isSupportedProvider)
			return BadRequest();

		var properties = new AuthenticationProperties {RedirectUri = ReturnUrl ?? "/"};

		return Challenge(properties, Provider);
	}

	private string GetProviderIconUrl(string providerSchemeName)
	{
		var iconPath = $"/external/{providerSchemeName}.png";
		var iconFileInfo = _fileProvider.GetFileInfo(iconPath);

		return iconFileInfo.Exists ? iconPath : "/external/Default.png";
	}
}