using Amphasis.Azure.Common.Models;
using AspNet.Security.OAuth.MailRu;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace Amphasis.Azure.WebPortal.Authentication;

public static class MailRuAuthenticationOptionsConfigurator
{
	public static void Configure(this MailRuAuthenticationOptions options, ConfigurationManager configuration)
	{
		configuration.Bind("MailRu", options);

		options.ClaimActions.MapJsonKey(CustomClaims.UserImageUrl, "image");
	}
}