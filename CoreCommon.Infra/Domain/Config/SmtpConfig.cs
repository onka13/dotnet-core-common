namespace CoreCommon.Infra.Domain.Config
{
    /// <summary>
    /// Email configuration model which defined in appsettings.
    /// </summary>
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
