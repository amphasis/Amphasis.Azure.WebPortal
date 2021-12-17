using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Amphasis.Azure.WebPortal.Models;
using Amphasis.Azure.WebPortal.SimaLand.Models;
using Amphasis.Azure.WebPortal.SimaLand.Services;
using Amphasis.Azure.WebPortal.Yandex.Models;
using Amphasis.SimaLand;
using AspNet.Security.OAuth.MailRu;
using AspNet.Security.OAuth.Vkontakte;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var applicationBuilder = WebApplication.CreateBuilder(args);
var configuration = applicationBuilder.Configuration;
var services = applicationBuilder.Services;

services.AddMemoryCache();
services.AddResponseCaching();
services.AddScoped<ImageProcessingService>();
services.AddScoped<SimaLandService>();
services.AddHttpClient<SimaLandService>();
services.AddHttpClient<SimaLandApiClient>();
services.Configure<SimaLandClientConfiguration>(configuration.GetSection("Simaland"));
services.Configure<YandexConfiguration>(configuration.GetSection("Yandex"));

services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

services
    .AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/SignIn";
        options.LogoutPath = "/SignOut";
    })
    .AddMailRu(options =>
    {
        configuration.GetSection("MailRu").Bind(options);

        options.Events.OnCreatingTicket += context =>
        {
            var identity = context.Identity;
            var originalImageClaim = identity?.FindFirst(MailRuAuthenticationConstants.Claims.ImageUrl);
            if (originalImageClaim == null) return Task.CompletedTask;
            identity.RemoveClaim(originalImageClaim);
            var newImageClaim = new Claim(CustomClaims.UserImageUrl, originalImageClaim.Value, originalImageClaim.ValueType);
            identity.AddClaim(newImageClaim);
            return Task.CompletedTask;
        };
    })
    .AddVkontakte(options =>
    {
        configuration.GetSection("VK").Bind(options);

        options.Events.OnCreatingTicket += context =>
        {
            var identity = context.Identity;
            var originalImageClaim = identity?.FindFirst(VkontakteAuthenticationConstants.Claims.PhotoUrl);
            if (originalImageClaim == null) return Task.CompletedTask;
            identity.RemoveClaim(originalImageClaim);
            var newImageClaim = new Claim(CustomClaims.UserImageUrl, originalImageClaim.Value, originalImageClaim.ValueType);
            identity.AddClaim(newImageClaim);
            return Task.CompletedTask;
        };
    })
    .AddYandex(options =>
    {
        configuration.GetSection("Yandex").Bind(options);

        options.Events.OnCreatingTicket += async context =>
        {
            if (context.Identity == null) return;
            var uri = new Uri("https://login.yandex.ru/info");
            var authorization = new AuthenticationHeaderValue("OAuth", context.AccessToken);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = authorization;
            using var httpResponseMessage = await httpClient.GetAsync(uri);
            httpResponseMessage.EnsureSuccessStatusCode();
            var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            var userInfo = JsonConvert.DeserializeObject<YandexUserInfo>(contentString);
            var imageUrl = $"https://avatars.yandex.net/get-yapic/{userInfo.DefaultAvatarId}/islands-200";
            var imageClaim = new Claim(CustomClaims.UserImageUrl, imageUrl, ClaimValueTypes.String);
            context.Identity.AddClaim(imageClaim);
        };
    });

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
