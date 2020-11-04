using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using Amphasis.Azure.WebPortal.Models;
using Amphasis.Azure.WebPortal.SimaLand.Models;
using Amphasis.Azure.WebPortal.SimaLand.Services;
using Amphasis.Azure.WebPortal.Yandex.Models;
using Amphasis.SimaLand;
using AspNet.Security.OAuth.MailRu;
using AspNet.Security.OAuth.Vkontakte;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Amphasis.Azure.WebPortal
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddScoped<ImageProcessingService>();
            services.AddScoped<SimaLandService>();
            services.AddHttpClient<SimaLandService>();
            services.AddHttpClient<SimaLandApiClient>();
            services.Configure<SimaLandClientConfiguration>(_configuration.GetSection("Simaland"));
            services.Configure<YandexConfiguration>(_configuration.GetSection("Yandex"));

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
                    _configuration.GetSection("MailRu").Bind(options);

                    options.Events.OnCreatingTicket += async context =>
                    {
                        var identity = context.Identity;
                        var imageClaim = identity.FindFirst(MailRuAuthenticationConstants.Claims.ImageUrl);
                        identity.RemoveClaim(imageClaim);
                        imageClaim = new Claim(CustomClaims.UserImageUrl, imageClaim.Value, imageClaim.ValueType);
                        identity.AddClaim(imageClaim);
                    };
                })
                .AddVkontakte(options =>
                {
                    _configuration.GetSection("VK").Bind(options);

                    options.Events.OnCreatingTicket += async context =>
                    {
                        var identity = context.Identity;
                        var imageClaim = identity.FindFirst(VkontakteAuthenticationConstants.Claims.PhotoUrl);
                        identity.RemoveClaim(imageClaim);
                        imageClaim = new Claim(CustomClaims.UserImageUrl, imageClaim.Value, imageClaim.ValueType);
                        identity.AddClaim(imageClaim);
                    };
                })
                .AddYandex(options =>
                {
                    _configuration.GetSection("Yandex").Bind(options);

                    options.Events.OnCreatingTicket += async context =>
                    {
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

            if (_environment.IsDevelopment())
            {
                razorPagesBuilder.AddRazorRuntimeCompilation();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                applicationBuilder.UseExceptionHandler("/Error");
                applicationBuilder.UseHsts();
            }

            applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseResponseCaching();
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseCookiePolicy();

            applicationBuilder.UseRouting();

            applicationBuilder.UseAuthentication();
            applicationBuilder.UseAuthorization();

            applicationBuilder.UseEndpoints(builder =>
            {
                builder.MapRazorPages();
                builder.MapControllers();
            });
        }
    }
}
