namespace Editor.Services
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Jwt { get; set; }
        public bool IsInactive { get; set; }
        public bool WrongCreditentials { get; set; }
    }
}
