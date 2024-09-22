using Amphasis.Azure.Common.Models;
using AspNet.Security.OAuth.Vkontakte;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace Amphasis.Azure.WebPortal.Authentication;

public static class VkontakteAuthenticationOptionsConfigurator
{
	public static void Configure(this VkontakteAuthenticationOptions options, ConfigurationManager configuration)
	{
		configuration.Bind("VK", options);

		options.ClaimActions.MapJsonKey(CustomClaims.UserImageUrl, "photo");
	}
}