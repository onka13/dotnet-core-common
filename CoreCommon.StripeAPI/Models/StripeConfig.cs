namespace CoreCommon.StripeAPI.Models
{
    public class StripeConfig
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
        public string WebhookSecret { get; set; }
    }
}
