using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Amphasis.Azure.WebPortal.Authentication;

public static class CookieAuthenticationOptionsConfigurator
{
	public static void Configure(this CookieAuthenticationOptions options, IHostApplicationBuilder applicationBuilder)
	{
		options.Cookie.HttpOnly = true;
		options.Cookie.SameSite = SameSiteMode.Lax;
		options.Cookie.SecurePolicy = applicationBuilder.Environment.IsDevelopment()
			? CookieSecurePolicy.None
			: CookieSecurePolicy.Always;
		options.LoginPath = "/SignIn";
	}
}