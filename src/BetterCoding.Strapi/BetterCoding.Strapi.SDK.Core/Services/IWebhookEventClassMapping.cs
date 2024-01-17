using BetterCoding.Strapi.SDK.Core.Webhook;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IWebhookEventClassMapping
    {
        string GetClassName<T>(T entity);
    }

    public class WebhookEventClassMapping : IWebhookEventClassMapping
    {
        public string GetClassName<T>(T entity) => entity switch
        {
            null => throw new NotSupportedException(),
            IWebhookPayload { } webhookPayload => webhookPayload.Model,
            _ => throw new NotSupportedException()
        };
    }
}
