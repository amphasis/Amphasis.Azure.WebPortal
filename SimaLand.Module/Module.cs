using Amphasis.Azure.SimaLand.Models;
using Amphasis.Azure.SimaLand.Services;
using Amphasis.SimaLand;

namespace Amphasis.Azure.SimaLand;

public static class Module
{
	public static void AddSimaLand(this IServiceCollection services, Action<SimaLandClientConfiguration> configure)
	{
		services.AddScoped<ImageProcessingService>();
		services.AddScoped<SimaLandService>();
		services.AddHttpClient<SimaLandService>();
		services.AddHttpClient<SimaLandApiClient>();
		services.Configure(configure);
	}
}