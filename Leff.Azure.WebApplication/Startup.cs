using Amphasis.Azure.WebPortal.Models;
using Amphasis.Azure.WebPortal.Services;
using Amphasis.SimaLand;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddScoped<ImageProcessingService>();
            services.AddScoped<SimaLandService>();
            services.AddHttpClient<SimaLandService>();
            services.AddHttpClient<SimaLandApiClient>();
            services.Configure<SimaLandClientConfiguration>(_configuration.GetSection("Simaland"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
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
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseCookiePolicy();

            applicationBuilder.UseRouting();
            applicationBuilder.UseEndpoints(builder =>
            {
                builder.MapRazorPages();
                builder.MapControllers();
            });
        }
    }
}
