﻿using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Amphasis.SimaLand;
using Leff.Azure.WebApplication.Extensions;
using Leff.Azure.WebApplication.Models;
using Leff.Azure.WebApplication.Models.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Leff.Azure.WebApplication.Services
{
    public class SimaLandService
    {
        private const string ApiClientAccessTokenKey = nameof(ApiClientAccessTokenKey);
        private static readonly TimeSpan TokenExpirationSkew = TimeSpan.FromSeconds(30);

        private readonly SimaLandClientConfiguration _configuration;
        private readonly SimaLandApiClient _apiClient;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;

        public SimaLandService(
            IOptions<SimaLandClientConfiguration> options,
            SimaLandApiClient apiClient,
            IMemoryCache memoryCache, 
            HttpClient httpClient)
        {
            _configuration = options.Value;
            _apiClient = apiClient;
            _memoryCache = memoryCache;
            _httpClient = httpClient;
        }

        public async Task<Stream> DownloadImageAsync(int itemId, int imageIndex, SimaLandImageSize imageSize)
        {
            if (itemId < 0) throw new ArgumentOutOfRangeException(nameof(itemId));
            if (imageIndex < 0) throw new ArgumentOutOfRangeException(nameof(imageIndex));
            
            if (!Enum.IsDefined(typeof(SimaLandImageSize), imageSize))
            {
                throw new InvalidEnumArgumentException(nameof(imageSize), (int) imageSize, typeof(SimaLandImageSize));
            }

            await AuthorizeAsync();
            var itemResponse = await _apiClient.GetItemAsync(itemId);
            var basePhotoUri = new Uri(itemResponse.BasePhotoUrl);
            var photoUri = new Uri(basePhotoUri, $"{imageIndex}/{imageSize.GetEnumMemberValue()}");

            var httpResponseMessage = await _httpClient.GetAsync(photoUri);
            httpResponseMessage.EnsureSuccessStatusCode();
            var imageStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            return imageStream;
        }

        private async ValueTask AuthorizeAsync()
        {
            string token = await _memoryCache.GetOrCreateAsync(ApiClientAccessTokenKey, TokenFactoryAsync);
            _apiClient.SetAccessToken(token);
        }

        private async Task<string> TokenFactoryAsync(ICacheEntry cacheEntry)
        {
            string token = await _apiClient.GetAccessTokenAsync(_configuration.Email, _configuration.Password);
            var jwtSecurityToken = new JwtSecurityToken(token);
            cacheEntry.AbsoluteExpiration = jwtSecurityToken.ValidTo - TokenExpirationSkew;

            return token;
        }

    }
}