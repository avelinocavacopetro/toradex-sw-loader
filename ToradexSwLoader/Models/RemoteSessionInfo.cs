namespace ToradexSwLoader.Models
{
    public class RemoteSessionInfo
    {
        public SshInfo Ssh { get; set; } = new();

        public class SshInfo
        {
            public List<string> AuthorizedPubKeys { get; set; } = new();
            public int ReversePort { get; set; }
            public string RaServerUrl { get; set; } = string.Empty;
            public string RaServerSshPubKey { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
        }
    }
}
