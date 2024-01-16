using BetterCoding.Strapi.SDK.Core.Entry;
using BetterCoding.Strapi.SDK.Core.Query;
using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core
{
    public class StrapiServerConfiguration
    {
        public string ServerURI { get; set; } = string.Empty;
        public string APIToken { get; set; } = string.Empty;
    }

    public class StrapiClient
    {
        public StrapiServerConfiguration ServerConfiguration { get; set; }
        public IServiceHub Services { get; internal set; }

        public StrapiClient(StrapiServerConfiguration configuration = default, IServiceHub serviceHub = default)
        {
            ServerConfiguration = configuration is null ? new StrapiServerConfiguration() : configuration;
            Services = serviceHub is null ? new ServiceHub(configuration) : serviceHub;
            Instance = this;
        }

        public static StrapiClient Instance { get; private set; }

        public QueryBuilder GetQueryBuilder()
        {
            return new QueryBuilder(Services);
        }

        public StrapiREST GetREST() 
        {
            return new StrapiREST(Services);
        }
    }
}
