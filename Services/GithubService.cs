using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Editor.Config;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Editor.Services
{
    public interface IGithubService
    {
        Task<GithubUserInfo> GetUserInfoAsync(string githubToken);
        Task<GithubAccessTokenResponse> GetAccessTokenAsync(string code);
    }

    public class GithubService : IGithubService
    {
        private readonly IErrorService errorService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly GithubOAuthOptions oauthOptions;

        public GithubService(IErrorService errorService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.errorService = errorService;
            this.httpClientFactory = httpClientFactory;
            oauthOptions = configuration.GetSection(nameof(GithubOAuthOptions)).Get<GithubOAuthOptions>();
        }

        public async Task<GithubUserInfo> GetUserInfoAsync(string githubToken)
        {
            var client = httpClientFactory.CreateClient("GithubApiClient");
            client.DefaultRequestHeaders
                  .Authorization = new AuthenticationHeaderValue("token", githubToken);
                  
            var response = await client.GetAsync("/user").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var userInfo = JsonConvert.DeserializeObject<GithubUserInfo>(await response.Content.ReadAsStringAsync());
                return userInfo;
            }

            return null;
        }

        public async Task<GithubAccessTokenResponse> GetAccessTokenAsync(string code)
        {
            var client = httpClientFactory.CreateClient("GithubClient");

            var requestUri = $"login/oauth/access_token?client_id={oauthOptions.ClientId}&redirect_uri={oauthOptions.RedirectUri}&client_secret={oauthOptions.ClientSecret}&code={code}";
            var response = await client.PostAsync(requestUri, null).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<GithubAccessTokenResponse>(await response.Content
                                                                                              .ReadAsStringAsync()
                                                                                              .ConfigureAwait(false));
        }
    }
}