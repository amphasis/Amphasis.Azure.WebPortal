using Microsoft.AspNetCore.Authentication;

namespace Amphasis.Azure.WebPortal.Authentication;

internal static class TestAuthentication
{
	public static AuthenticationBuilder AddTestAuthentication(this AuthenticationBuilder builder)
	{
		return builder.AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
			authenticationScheme: "Test",
			displayName: "Test Authentication",
			configureOptions: null);
	}
}