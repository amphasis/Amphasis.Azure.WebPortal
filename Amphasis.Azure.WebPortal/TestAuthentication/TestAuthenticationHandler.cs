using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amphasis.Azure.WebPortal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amphasis.Azure.WebPortal.TestAuthentication;

internal sealed class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public TestAuthenticationHandler(
		IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder)
		: base(options, logger, encoder)
	{
	}

	protected override Task HandleChallengeAsync(AuthenticationProperties properties)
	{
		return AuthenticateAsync();
	}

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, "TestUserId"),
			new Claim(ClaimTypes.Name, "Test User"),
			new Claim(ClaimTypes.Email, "test@mailforspam.com"),
			new Claim(ClaimTypes.Authentication, "true"),
			new Claim(CustomClaims.UserImageUrl, "/external/Default.png"),
		};

		var identity = new ClaimsIdentity(claims, Scheme.Name);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, Scheme.Name);

		await Context.SignInAsync(principal);

		return AuthenticateResult.Success(ticket);
	}
}
