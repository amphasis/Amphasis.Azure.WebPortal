using System.Security.Claims;
using System.Threading.Tasks;
using Amphasis.Azure.Common.Models;
using AspNet.Security.OAuth.Vkontakte;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;

namespace Amphasis.Azure.WebPortal.Authentication;

public static class VkontakteAuthenticationOptionsConfigurator
{
	public static void ConfigureOptions(ConfigurationManager configuration, VkontakteAuthenticationOptions options)
	{
		configuration.Bind("VK", options);
		options.Events.OnCreatingTicket += onCreatingTicket;
	}

	private static Task onCreatingTicket(OAuthCreatingTicketContext context)
	{
		var identity = context.Identity;
		var originalImageClaim = identity?.FindFirst(VkontakteAuthenticationConstants.Claims.PhotoUrl);

		if (identity == null || originalImageClaim == null)
		{
			return Task.CompletedTask;
		}

		identity.RemoveClaim(originalImageClaim);

		var newImageClaim = new Claim(
			CustomClaims.UserImageUrl,
			originalImageClaim.Value,
			originalImageClaim.ValueType);

		identity.AddClaim(newImageClaim);

		return Task.CompletedTask;
	}
}