using Amphasis.Azure.SimaLand;
using Amphasis.Azure.WebPortal.Authentication;
using Amphasis.Azure.Yandex.Models;
using Amphasis.Azure.Yandex.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var applicationBuilder = WebApplication.CreateBuilder(args);
var configuration = applicationBuilder.Configuration;
var services = applicationBuilder.Services;

services.AddMemoryCache();
services.AddResponseCaching();
services.AddSimaLand(options => configuration.Bind("Simaland", options));
services.Configure<YandexConfiguration>(configuration.GetSection("Yandex"));

services.Configure<CookiePolicyOptions>(options =>
{
	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
	options.CheckConsentNeeded = _ => true;
	options.MinimumSameSitePolicy = SameSiteMode.None;
});

var authenticationBuilder = services
	.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => options.LoginPath = "/SignIn")
	.AddMailRu(options => MailRuAuthenticationOptionsConfigurator.ConfigureOptions(configuration, options))
	.AddVkontakte(options => VkontakteAuthenticationOptionsConfigurator.ConfigureOptions(configuration, options))
	.AddYandex(options => YandexAuthenticationOptionsConfigurator.ConfigureOptions(configuration, options));

if (applicationBuilder.Environment.IsDevelopment())
{
	authenticationBuilder.AddTestAuthentication();
}

var razorPagesBuilder = services.AddRazorPages();
services.AddControllers();

if (applicationBuilder.Environment.IsDevelopment())
{
	razorPagesBuilder.AddRazorRuntimeCompilation();
}

var application = applicationBuilder.Build();

if (application.Environment.IsDevelopment())
{
	application.UseDeveloperExceptionPage();
}
else
{
	application.UseExceptionHandler("/Error");
	application.UseHsts();
}

application.UseHttpsRedirection();
application.UseResponseCaching();
application.UseStaticFiles();
application.UseCookiePolicy();

application.UseRouting();

application.UseAuthentication();
application.UseAuthorization();

application.MapRazorPages();
application.MapControllers();

application.Run();
