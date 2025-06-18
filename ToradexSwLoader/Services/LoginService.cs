namespace ToradexSwLoader.Services
{
    public class LoginService
    {
        public int UserId { get; private set; }
        public string Username { get; private set; } = string.Empty;

        public event Action? OnLoginChanged;

        public void SetLogin(int userId, string username)
        {
            UserId = userId;
            Username = username;
            OnLoginChanged?.Invoke();
        }
    }
}
