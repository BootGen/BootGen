using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Editor.Services;
using Editor.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Editor.Controllers
{
    [Route("oauth")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IRegistrationService registrationService;
        private readonly IGithubService githubService;
        private readonly IOAuthService oAuthService;

        public OAuthController(IAuthenticationService authenticationService, IRegistrationService registrationService, IGithubService githubService, IOAuthService oAuthService)
        {
            this.authenticationService = authenticationService;
            this.registrationService = registrationService;
            this.githubService = githubService;
            this.oAuthService = oAuthService;
        }

        [HttpPost]
        [Route("github/login")]
        public async Task<IActionResult> GithubLogin([FromBody] OAuthLoginData data)
        {
            var tokenInfo = await githubService.GetAccessTokenAsync(data.Code).ConfigureAwait(false);
            if (string.IsNullOrEmpty(tokenInfo.Error))
            {
                var githubUserInfo = await githubService.GetUserInfoAsync(tokenInfo.AccessToken).ConfigureAwait(false);
                var githubUser = oAuthService.GetGithubUser(githubUserInfo.Id);
                if (githubUser == null)
                {
                    var githubRegistrationData = new GithubRegistrationData
                                                 {
                                                     GithubToken = tokenInfo.AccessToken
                                                 };
                    githubUser = await registrationService.RegisterViaGithub(githubRegistrationData, githubUserInfo);
                }

                var loginResponse = authenticationService.GithubOAuthLogin(githubUser, tokenInfo.AccessToken);
                if (loginResponse.Success)
                {
                    return Ok(loginResponse);
                }
            }
            return BadRequest();
        }
    }

    public class OAuthLoginData
    {
        public string Code { get; set; }
    }
}