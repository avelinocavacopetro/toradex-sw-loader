namespace ToradexSwLoader.Services
{
    public class LoginService
    {
        public int CurrentLoginId { get; private set; }
        public string Email { get; private set; }

        public event Action? OnLoginChanged;

        public void SetLogin(int loginId, string email)
        {
            CurrentLoginId = loginId;
            Email = email;
            OnLoginChanged?.Invoke();
        }
    }


}
