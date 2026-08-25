using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace ITOWebApiClient
{
    public class ApiTokenCacheClient
    {
        private readonly ILogger<ApiTokenCacheClient> _logger;
        private readonly HttpClient _httpClient;
        private static readonly Object _lock = new Object();
        private IDistributedCache _cache;

        private const int cacheExpirationInDays = 1;

        private class AccessTokenItem
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresIn { get; set; }
        }

        public ApiTokenCacheClient(
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory,
            IDistributedCache cache)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = loggerFactory.CreateLogger<ApiTokenCacheClient>();
            _cache = cache;
        }

        public async Task<string> GetApiToken(string api_name, string api_scope, string secret,string username, string password)
        {
            var accessToken = GetFromCache(api_name);

            if (accessToken != null)
            {
                if (accessToken.ExpiresIn > DateTime.UtcNow)
                {
                    return accessToken.AccessToken;
                }
                else
                {
                    // remove  => NOT Needed for this cache type
                }
            }

            _logger.LogDebug($"GetApiToken new from STS for {api_name}");

            // add
            var newAccessToken = await getApiToken( api_name,  api_scope,  secret, username, password);
            AddToCache(api_name, newAccessToken);

            return newAccessToken.AccessToken;
        }

        private async Task<AccessTokenItem> getApiToken(string api_name, string api_scope, string secret, string username, string password)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
               .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"), optional: false)
               .Build();

                var authUrl = configuration.GetValue<string>("ApiResourceBaseUrls:AuthServer");

                var disco = _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
                {
                    Address = authUrl,
                    Policy = { RequireHttps = false }
                }).Result;

                if (disco.IsError)
                {
                    _logger.LogError($"disco error Status code: {disco.IsError}, Error: {disco.Error}");
                    throw new ApplicationException($"Status code: {disco.IsError}, Error: {disco.Error}");
                }

                var tokenResponse = await HttpClientTokenRequestExtensions.RequestPasswordTokenAsync(_httpClient, new PasswordTokenRequest
                {
                    Scope = api_scope,
                    ClientSecret = secret,
                    Address = disco.TokenEndpoint,
                    ClientId = api_name,
                    UserName = username,
                    Password = password
                });

                if (tokenResponse.IsError)
                {
                    _logger.LogError($"tokenResponse.IsError Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
                    throw new ApplicationException($"Status code: {tokenResponse.IsError}, Error: {tokenResponse.Error}");
                }

                return new AccessTokenItem
                {
                    ExpiresIn = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    AccessToken = tokenResponse.AccessToken
                };
                
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception {e}");
                throw new ApplicationException($"Exception {e}");
            }
        }

        private void AddToCache(string key, AccessTokenItem accessTokenItem)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(cacheExpirationInDays));

            lock (_lock)
            {
                _cache.SetString(key, System.Text.Json.JsonSerializer.Serialize(accessTokenItem), options);
            }
        }

        private AccessTokenItem GetFromCache(string key)
        {
            var item = _cache.GetString(key);
            if (item != null)
            {
                return System.Text.Json.JsonSerializer.Deserialize<AccessTokenItem>(item);
            }

            return null;
        }
    }
}
