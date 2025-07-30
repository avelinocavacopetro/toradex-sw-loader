namespace ToradexSwLoader.Models
{
    public class RemoteSessionInfo
    {
        public SshInfo Ssh { get; set; }

        public class SshInfo
        {
            public List<string> AuthorizedPubKeys { get; set; }
            public int ReversePort { get; set; }
            public string RaServerUrl { get; set; }
            public string RaServerSshPubKey { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}
