using BetterCoding.Strapi.SDK.Core.Entry;
using BetterCoding.Strapi.SDK.Core.Http;
using BetterCoding.Strapi.SDK.Core.Utilities;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IStrapiREST
    {
        HttpRequest CreateRequest(
           string relativeUri,
           string method,
           IDictionary<string, object> data = null);

        Task<IDictionary<string, object>> ExecuteAsync(HttpRequest request);

        Task<IDictionary<string, object>> ExecuteAsync(
           string relativeUri,
           string method,
           IDictionary<string, object> data = null);
    }

    public class StrapiREST : IStrapiREST
    {
        public string APIPath = "api";
        public IServiceHub Services { get; internal set; }

        public StrapiREST(IServiceHub serviceHub = default)
        {
            Services = serviceHub;
        }

        public HttpRequest CreateRequest(
            string relativeUri,
            string method,
            IDictionary<string, object> data = null)
        {
            var token = Services.Server.ActiveUser != null ? Services.Server.ActiveUser.Jwt : Services.Server.ServerConfiguration.APIToken;
            var presetHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            };

            var request = new HttpRequest(relativeUri, method,
               data: data,
               headers: presetHeaders.ToList())
            {
                Resource = Services.Server.ServerConfiguration.ServerURI
            };

            return request;
        }

        public async Task<IDictionary<string, object>> ExecuteAsync(HttpRequest request)
        {
            var response = await Services.WebClient.ExecuteAsync(request);
            var content = response.Item2;
            var statusCode = (int)response.Item1;
            if (statusCode < 200 || statusCode > 299)
            {
                throw new HttpRequestException();
            }

            var contentJson = Services.JsonTool.Parse(content, Services);
            return contentJson;
        }

        public async Task<IDictionary<string, object>> ExecuteAsync(
            string relativeUri,
            string method,
            IDictionary<string, object> data = null)
        {
            var request = CreateRequest($"{relativeUri}", method, data);
            return await ExecuteAsync(request);
        }
    }
}
