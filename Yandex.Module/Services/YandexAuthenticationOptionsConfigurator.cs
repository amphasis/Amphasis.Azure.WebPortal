using System.Net.Http.Headers;
using System.Security.Claims;
using Amphasis.Azure.Common.Models;
using Amphasis.Azure.Yandex.Models;
using AspNet.Security.OAuth.Yandex;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;

namespace Amphasis.Azure.Yandex.Services;

public static class YandexAuthenticationOptionsConfigurator
{
	public static void ConfigureOptions(ConfigurationManager configuration, YandexAuthenticationOptions options)
	{
		configuration.Bind("Yandex", options);
		options.Events.OnCreatingTicket += onCreatingTicket;
	}

	private static async Task onCreatingTicket(OAuthCreatingTicketContext context)
	{
		if (context.Identity == null)
		{
			return;
		}

		using var request = new HttpRequestMessage(HttpMethod.Get, "https://login.yandex.ru/info");
		request.Headers.Authorization = new AuthenticationHeaderValue("OAuth", context.AccessToken);

		using var httpResponseMessage = await context.Backchannel.SendAsync(request);
		httpResponseMessage.EnsureSuccessStatusCode();

		var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
		var userInfo = JsonConvert.DeserializeObject<YandexUserInfo>(contentString);

		if (userInfo == null)
		{
			throw new AuthenticationFailureException("Unable to deserialize yandex user info.");
		}

		var imageUrl = $"https://avatars.yandex.net/get-yapic/{userInfo.DefaultAvatarId}/islands-200";
		var imageClaim = new Claim(CustomClaims.UserImageUrl, imageUrl, ClaimValueTypes.String);
		context.Identity.AddClaim(imageClaim);
	}
}