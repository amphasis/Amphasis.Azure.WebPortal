using System.Security.Claims;
using System.Threading.Tasks;
using Amphasis.Azure.Common.Models;
using AspNet.Security.OAuth.MailRu;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;

namespace Amphasis.Azure.WebPortal.Authentication;

public static class MailRuAuthenticationOptionsConfigurator
{
	public static void ConfigureOptions(ConfigurationManager configuration, MailRuAuthenticationOptions options)
	{
		configuration.Bind("MailRu", options);
		options.Events.OnCreatingTicket += onCreatingTicket;
	}

	private static Task onCreatingTicket(OAuthCreatingTicketContext context)
	{
		var identity = context.Identity;
		var originalImageClaim = identity?.FindFirst(MailRuAuthenticationConstants.Claims.ImageUrl);

		if (originalImageClaim != null)
		{
			identity.RemoveClaim(originalImageClaim);

			var newImageClaim = new Claim(
				CustomClaims.UserImageUrl,
				originalImageClaim.Value,
				originalImageClaim.ValueType);

			identity.AddClaim(newImageClaim);
		}

		return Task.CompletedTask;
	}
}