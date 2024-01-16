// Ignore Spelling: Strapi

using BetterCoding.Strapi.SDK.Core.Http;
using BetterCoding.Strapi.SDK.Core.Webhook;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IServiceHub
    {
        IDataDecoder DataDecoder { get; }
        IWebhookEventCoder WebhookEventCoder { get; }
        IWebhookEventClassMapping WebhookEventClassMapping { get; }
        IQueryStringEncoder QueryStringEncoder { get; }
        IWebClient WebClient { get; }
        StrapiServerConfiguration ServerConfiguration { get; }
        IJsonTool JsonTool { get; }
        IEntryStateCoder EntryStateCoder { get; }
        IEntryController EntryController { get; }
    }

    public class ServiceHub : IServiceHub
    {
        public ServiceHub(StrapiServerConfiguration strapiServerConfiguration)
        {
            ServerConfiguration = strapiServerConfiguration;
        }
        public IDataDecoder DataDecoder => new DataDecoder();
        public IWebhookEventCoder WebhookEventCoder => new WebhookEventCoder();
        public IWebhookEventClassMapping WebhookEventClassMapping => new WebhookEventClassMapping();
        public IQueryStringEncoder QueryStringEncoder => new QueryStringEncoder();
        public IWebClient WebClient => new UniversalWebClient();
        public StrapiServerConfiguration ServerConfiguration { get; internal set; }
        public IJsonTool JsonTool => new JsonTool();
        public IEntryStateCoder EntryStateCoder => new EntryStateCoder();
        public IEntryController EntryController => new EntryController();
    }
}
