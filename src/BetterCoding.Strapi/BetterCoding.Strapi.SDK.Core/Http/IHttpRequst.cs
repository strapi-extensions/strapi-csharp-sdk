using BetterCoding.Strapi.SDK.Core.Services;
using System.Text;

namespace BetterCoding.Strapi.SDK.Core.Http
{
    public interface IHttpRequst
    {
        Uri Target { get; }

        string Resource { get; set; }

        string Path { get; set; }

        IList<KeyValuePair<string, string>> Headers { get; set; }

        Stream Data { get; set; }

        string Method { get; set; }
    }

    public class HttpRequest : IHttpRequst
    {
        public Uri Target => new Uri(new Uri(Resource), Path);

        public string Resource { get; set; }

        public string Path { get; set; }

        public IList<KeyValuePair<string, string>> Headers { get; set; }

        public IDictionary<string, object> DataObject { get; private set; }
        public virtual Stream Data { get; set; }
        public string Method { get; set; }

        public HttpRequest(string relativeUri,
            string method, string sessionToken = null,
            IList<KeyValuePair<string, string>> headers = null,
            IDictionary<string, object> data = null) : this(
                relativeUri: relativeUri,
                method: method,
                sessionToken: sessionToken,
                headers: headers,
                stream: data is { } ? new MemoryStream(Encoding.UTF8.GetBytes(JsonUtilities.Encode(data))) : default,
                contentType: data != null ? "application/json" : null)
        {
            
        }

        public HttpRequest(string relativeUri,
            string method,
            string sessionToken = null,
            IList<KeyValuePair<string, string>> headers = null,
            Stream stream = null, string contentType = null)
        {
            Path = relativeUri;
            Method = method;
            Data = stream;
            Headers = new List<KeyValuePair<string, string>>(headers ?? Enumerable.Empty<KeyValuePair<string, string>>());

            if (!String.IsNullOrEmpty(sessionToken))
            {
                Headers.Add(new KeyValuePair<string, string>("Authorization", sessionToken));
            }

            if (!String.IsNullOrEmpty(contentType))
            {
                Headers.Add(new KeyValuePair<string, string>("Content-Type", contentType));
            }
        }

        public HttpRequest(HttpRequest other)
        {
            Resource = other.Resource;
            Path = other.Path;
            Method = other.Method;
            DataObject = other.DataObject;
            Headers = new List<KeyValuePair<string, string>>(other.Headers);
            Data = other.Data;
        }
    }
}
