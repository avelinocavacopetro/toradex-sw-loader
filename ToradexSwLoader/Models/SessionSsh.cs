namespace ToradexSwLoader.Models
{
    public class SessionSsh
    {
        public List<string> AuthorizedPubKeys { get; set; } = new();
        public int ReversePort { get; set; }
        public string RaServerUrl { get; set; } = string.Empty;
        public string RaServerSshPubKey { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
