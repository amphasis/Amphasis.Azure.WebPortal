using System;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Amphasis.Azure.WebPortal.Helpers;

public static class JwtHelper
{
	public static DateTime GetValidTo(string encodedJsonWebToken)
	{
		var destination = new Span<Range>(new Range[3]);
		var partsCount = encodedJsonWebToken.AsSpan().Split(destination, '.');

		if (partsCount < 3)
			throw new JwtHelperException("Could not extract payload");

		var payloadRange = destination[1];
		var (offset, count) = payloadRange.GetOffsetAndLength(encodedJsonWebToken.Length);
		var bytes = WebEncoders.Base64UrlDecode(encodedJsonWebToken, offset, count);
		var payload = JsonSerializer.Deserialize<JwtPayload>(bytes, _jsonSerializerOptions);

		return DateTime.UnixEpoch.AddSeconds(payload.Exp);
	}

	static JwtHelper()
	{
		_jsonSerializerOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
	}

	private static readonly JsonSerializerOptions _jsonSerializerOptions;

	private sealed class JwtHelperException : Exception
	{
		public JwtHelperException(string message) : base(message)
		{
		}
	}

	private sealed record JwtPayload(int Exp);
}
