const clientId = process.env.VUE_APP_GITHUB_API_CLIENT_ID;
const redirectUri = process.env.VUE_APP_GITHUB_API_AUTH_REDIRECT_URI;


const githubApi = {
  authorize:  function () {
    window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&scope=user`;
  }
}

export default githubApi;


