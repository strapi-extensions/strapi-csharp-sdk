// Ignore Spelling: Webhook Strapi

namespace BetterCoding.Strapi.SDK.Core.Webhook
{
    public interface IWebhookPayload
    {
        string Event { get; set; }
        DateTime? CreatedAt { get; set; }
        string Model { get; set; }
        Dictionary<string, object> Entry { get; set; }
    }

    public class WebhookPayload: IWebhookPayload
    {
        public string Event { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Model { get; set; }
        public Dictionary<string, object> Entry { get; set; }
    }
}