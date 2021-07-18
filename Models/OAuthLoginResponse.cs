using Editor.Services;

namespace Editor
{
    public class OAuthLoginResponse : LoginResponse
    {
        public string AccessToken { get; set; }
    }
}