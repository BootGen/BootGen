namespace Editor
{
    public class GithubUser
    {
        public int GithubId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
    }
}