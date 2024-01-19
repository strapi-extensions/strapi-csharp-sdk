// Ignore Spelling: Strapi

using BetterCoding.Strapi.SDK.Core.Entry;
using BetterCoding.Strapi.SDK.Core.Http;
using BetterCoding.Strapi.SDK.Core.Server;
using BetterCoding.Strapi.SDK.Core.Webhook;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IServiceHub
    {
        IDataDecoder DataDecoder { get; }
        IWebhookEventCoder WebhookEventCoder { get; }
        IWebhookEventClassMapping WebhookEventClassMapping { get; }
        IQueryStringEncoder QueryStringEncoder { get; }
        IWebClient WebClient { get; set; }
        StrapiServer Server { get; }
        IJsonTool JsonTool { get; }
        IEntryStateCoder EntryStateCoder { get; }
        IEntryController EntryController { get; }
        IAttributeEncoder AttributeEncoder { get; }
        IStrapiEntryClassController ClassController { get; }
        IStrapiREST StrapiREST { get; }
        IUserStateCoder LocalUserStateCoder {  get; }
    }

    public class ServiceHub : IServiceHub
    {
        public ServiceHub(StrapiServer server)
        {
            Server = server;
            WebClient ??= new UniversalWebClient();
        }

        public IDataDecoder DataDecoder => new DataDecoder();
        public IWebhookEventCoder WebhookEventCoder => new WebhookEventCoder();
        public IWebhookEventClassMapping WebhookEventClassMapping => new WebhookEventClassMapping();
        public IQueryStringEncoder QueryStringEncoder => new QueryStringEncoder();
        public IWebClient WebClient { get; set; }
        public StrapiServer Server { get; internal set; }
        public IJsonTool JsonTool => new JsonTool();
        public IEntryStateCoder EntryStateCoder => new EntryStateCoder();
        public IEntryController EntryController => new EntryController();
        public IAttributeEncoder AttributeEncoder => new AttributeEncoder();
        public IStrapiEntryClassController ClassController => new StrapiEntryClassController();
        public IStrapiREST StrapiREST => new StrapiREST(this);
        public IUserStateCoder LocalUserStateCoder => new LocalUserStateCoder();
    }
}
