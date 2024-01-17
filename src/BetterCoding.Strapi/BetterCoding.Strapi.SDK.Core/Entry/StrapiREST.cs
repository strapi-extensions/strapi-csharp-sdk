using BetterCoding.Strapi.SDK.Core.Http;
using BetterCoding.Strapi.SDK.Core.Services;
using BetterCoding.Strapi.SDK.Core.Utilities;

namespace BetterCoding.Strapi.SDK.Core.Entry
{
    public class StrapiREST
    {
        public string APIPath = "api";
        public IServiceHub Services { get; internal set; }

        public StrapiREST(IServiceHub serviceHub = default)
        {
            Services = StrapiClient.GetClient().Services;
        }

        public async Task<StrapiEntry> Get(string entryName, int id)
        {
            var serverState = await ExecuteAsync($"{APIPath}/{entryName}/{id}", "GET").OnSuccess(task =>
            task.Result["data"] as IDictionary<string, object> is Dictionary<string, object> item && item != null ? Services.EntryStateCoder.Decode(item, Services.DataDecoder, Services) : null);
            if (serverState == null) throw new EntryPointNotFoundException();
            var entry = Services.EntryController.Create(entryName, serverState);
            return entry;
        }

        public async Task<IEnumerable<StrapiEntry>> Filtering(string entryName, string pluralApiId, string filters)
        {
            var serverStates = await ExecuteAsync($"{APIPath}/{pluralApiId}?{filters}", "GET").OnSuccess(task =>
            (from item in task.Result["data"] as IList<object> select Services.EntryStateCoder.Decode(item as IDictionary<string, object>, Services.DataDecoder, Services)));
            var result = serverStates.Select(s => Services.EntryController.Create(entryName, s)).ToList();
            return result;
        }

        public HttpRequest CreateRequest(
            string relativeUri,
            string method,
            IDictionary<string, object> data = null)
        {
            var presetHeaders = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {Services.ServerConfiguration.APIToken}" }
            };

            var request = new HttpRequest(relativeUri, method,
               data: data,
               headers: presetHeaders.ToList())
            {
                Resource = Services.ServerConfiguration.ServerURI
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
