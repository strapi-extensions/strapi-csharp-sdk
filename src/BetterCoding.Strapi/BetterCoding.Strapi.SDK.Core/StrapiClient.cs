using BetterCoding.Strapi.SDK.Core.Entry;
using BetterCoding.Strapi.SDK.Core.Query;
using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core
{
    public class StrapiServerConfiguration
    {
        public string ServerURI { get; set; } = string.Empty;
        public string APIToken { get; set; } = string.Empty;

        /// <summary>
        /// key to map server
        /// </summary>
        public string Alias { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }

    public class StrapiClient
    {
        public StrapiServerConfiguration ServerConfiguration { get; set; }
        public IServiceHub Services { get; internal set; }

        public StrapiClient(StrapiServerConfiguration configuration = default, IServiceHub serviceHub = default)
        {
            ServerConfiguration = configuration is null ? new StrapiServerConfiguration() : configuration;
            Services = serviceHub is null ? new ServiceHub(configuration) : serviceHub;
        }

        private static IDictionary<string, StrapiServerConfiguration> _servers = new Dictionary<string, StrapiServerConfiguration>();
        private static string _currentAlias = string.Empty;

        public static StrapiClient AddServer(StrapiServerConfiguration serverConfigurations) 
        {
            return AddServers(new List<StrapiServerConfiguration> { serverConfigurations });
        }

        public static StrapiClient AddServers(params StrapiServerConfiguration[] serverConfigurations)
        {
            if (!serverConfigurations.Any()) throw new ArgumentException("serverConfigurations can not be null or empty.");
            return AddServers(serverConfigurations.ToList());
        }

        public static StrapiClient AddServers(IEnumerable<StrapiServerConfiguration> serverConfigurations)
        {
            if (!serverConfigurations.Any()) throw new ArgumentException("serverConfigurations can not be null or empty.");

            if (!serverConfigurations.Any(s => s.IsDefault))
                serverConfigurations.FirstOrDefault().IsDefault = true;

            _servers = serverConfigurations.ToDictionary(x => x.Alias, x => x);
            return GetClient();
        }

        public static StrapiClient SwicthServer(string alias)
        {
            if (string.IsNullOrEmpty(alias)) throw new NotSupportedException("alias can not be null or empty");
            _currentAlias = alias;
            return GetClient(_currentAlias);
        }

        public static StrapiClient GetClient(string alias = "")
        {
            if (!_servers.Any()) throw new KeyNotFoundException("no server config found.");
            if (string.IsNullOrEmpty(alias))
            {
                if (!string.IsNullOrEmpty(_currentAlias))
                {
                    return GetClient(_currentAlias);
                }

                var defaultAlias = _servers.FirstOrDefault(s => s.Value.IsDefault);
                if (!defaultAlias.Equals(new KeyValuePair<string, StrapiServerConfiguration>()))
                {
                    return new StrapiClient(defaultAlias.Value);
                }
                var fisrt = _servers.FirstOrDefault();
                return new StrapiClient(fisrt.Value);
            }

            if (_servers.TryGetValue(alias, out var config))
            {
                return new StrapiClient(config);
            }

            throw new KeyNotFoundException($"no alias found alias name is {alias}");
        }

        public FiltersBuilder GetQueryBuilder()
        {
            return new FiltersBuilder(Services);
        }

        public StrapiREST GetREST()
        {
            return new StrapiREST(Services);
        }
    }
}
