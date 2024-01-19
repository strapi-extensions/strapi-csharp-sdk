using BetterCoding.Strapi.SDK.Core.Query;
using BetterCoding.Strapi.SDK.Core.Server;
using BetterCoding.Strapi.SDK.Core.Services;
using BetterCoding.Strapi.SDK.Core.User;

namespace BetterCoding.Strapi.SDK.Core
{
    public class StrapiClient
    {
        public IServiceHub Services { get; internal set; }

        public StrapiClient(StrapiServerConfiguration configuration = default, IServiceHub serviceHub = default)
        {
            var server = new StrapiServer(configuration);
            Services = serviceHub is null ? new ServiceHub(server) : serviceHub;
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

        public FiltersBuilder GetFiltersBuilder()
        {
            return new FiltersBuilder(Services);
        }

        public AuthBuilder GetAuthBuilder()
        {
            return new AuthBuilder(Services);
        }

        public IStrapiREST GetREST()
        {
            return Services.StrapiREST;
        }
    }
}
