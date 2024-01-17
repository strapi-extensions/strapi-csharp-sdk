using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core.Webhook
{
    public interface IWebhookEventCoder
    {
        IWebhookPayload Decode(IDictionary<string, object> data, IDataDecoder decoder, IServiceHub serviceHub);
    }

    public class WebhookEventCoder : IWebhookEventCoder
    {
        public IWebhookPayload Decode(IDictionary<string, object> data, IDataDecoder decoder, IServiceHub serviceHub)
        {
            Dictionary<string, object> serverData = new Dictionary<string, object> { },
                mutableData = new Dictionary<string, object>(data);

            string eventName = Extract(mutableData, "event", (obj) => obj as string);
            DateTime? createdAt = Extract<DateTime?>(mutableData, "createdAt", (obj) => DataDecoder.ParseDate(obj as string));
            string model = Extract(mutableData, "model", (obj) => obj as string);

            foreach (KeyValuePair<string, object> pair in mutableData)
            {
                serverData[pair.Key] = decoder.Decode(pair.Value, serviceHub);
            }

            return new WebhookPayload
            {
                Event = eventName,
                CreatedAt = createdAt,
                Model = model,
                Entry = serverData,
            };
        }

        T Extract<T>(IDictionary<string, object> data, string key, Func<object, T> action)
        {
            T result = default;

            if (data.ContainsKey(key))
            {
                result = action(data[key]);
                data.Remove(key);
            }

            return result;
        }
    }
}
