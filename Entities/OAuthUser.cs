namespace Editor
{
    public class OAuthUser
    {
        public int OAuthId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
    }
}