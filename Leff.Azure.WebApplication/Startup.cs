using System;
using Amphasis.SimaLand;
using Leff.Azure.WebApplication.Controllers;
using Leff.Azure.WebApplication.Models;
using Leff.Azure.WebApplication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Leff.Azure.WebApplication
{
    public class Startup
    {
        private readonly Uri _baseImageUri = new Uri("https://cdn2.static1-sima-land.com/items/");
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMemoryCache();
            services.AddScoped<ImageProcessingService>();
            services.AddScoped<SimaLandService>();
            services.AddHttpClient<SimaLandService>();
            services.AddHttpClient<SimaLandApiClient>();
            services.Configure<SimaLandClientConfiguration>(_configuration.GetSection("Simaland"));

            services.AddHttpClient(nameof(SimaLandImageController), c => c.BaseAddress = _baseImageUri);

            services.AddControllers();
            var mvcBuilder = services.AddRazorPages();
            if (_environment.IsDevelopment()) mvcBuilder.AddRazorRuntimeCompilation();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                applicationBuilder.UseHsts();
            }

            applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseCookiePolicy();

            applicationBuilder.UseRouting();
            applicationBuilder.UseEndpoints(builder =>
            {
                builder.MapControllers();
                builder.MapRazorPages();
            });
        }
    }
}
