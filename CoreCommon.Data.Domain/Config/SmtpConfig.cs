namespace CoreCommon.Data.Domain.Config
{
    public class SmtpConfig
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; }
    }
}
