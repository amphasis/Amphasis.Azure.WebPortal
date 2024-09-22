using System.Security.Claims;
using System.Text.Json;
using Amphasis.Azure.Common.Models;
using AspNet.Security.OAuth.Yandex;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.Extensions.Configuration;

namespace Amphasis.Azure.WebPortal.Authentication;

public static class YandexAuthenticationOptionsConfigurator
{
	public static void Configure(this YandexAuthenticationOptions options, ConfigurationManager configuration)
	{
		configuration.Bind("Yandex", options);

		options.ClaimActions.Add(new UserImageClaimAction());
	}

	private sealed class UserImageClaimAction : ClaimAction
	{
		public UserImageClaimAction() : base(CustomClaims.UserImageUrl, ClaimValueTypes.String)
		{
		}

		public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
		{
			var defaultAvatarId =
				userData.TryGetProperty("default_avatar_id", out var value) && value.ValueKind == JsonValueKind.String
					? value.ToString()
					: null;

			if (string.IsNullOrWhiteSpace(defaultAvatarId))
			{
				return;
			}

			var imageUrl = $"https://avatars.yandex.net/get-yapic/{defaultAvatarId}/islands-200";
			var imageClaim = new Claim(ClaimType, imageUrl, ValueType);
			identity.AddClaim(imageClaim);
		}
	}
}