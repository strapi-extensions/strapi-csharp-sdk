using BetterCoding.Strapi.SDK.Core.Entry;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IEntryStateCoder
    {
        IEntryState Decode(IDictionary<string, object> data, IDataDecoder decoder, IServiceHub serviceHub);
    }

    public class EntryStateCoder : IEntryStateCoder
    {
        public IEntryState Decode(IDictionary<string, object> data, IDataDecoder decoder, IServiceHub serviceHub)
        {
            IDictionary<string, object> serverData = new Dictionary<string, object> { },
                mutableData = new Dictionary<string, object>(data);

            int id = Extract(mutableData, "id", (obj) => int.Parse(obj.ToString()));
            var attributes = Extract<Dictionary<string, object>>(mutableData, "attributes", obj => obj as Dictionary<string, object>);
            DateTime? createdAt = Extract<DateTime?>(attributes, "createdAt", (obj) => DataDecoder.ParseDate(obj as string)),
                updatedAt = Extract<DateTime?>(attributes, "updatedAt", (obj) => DataDecoder.ParseDate(obj as string)),
                publishedAt = Extract<DateTime?>(attributes, "publishedAt", (obj) => obj == null ? null : DataDecoder.ParseDate(obj as string));

            if (createdAt != null && updatedAt == null)
            {
                updatedAt = createdAt;
            }

            foreach (KeyValuePair<string, object> pair in attributes)
            {
                serverData[pair.Key] = decoder.Decode(pair.Value, serviceHub);
            }

            return new MutableEntryState
            {
                Id = id,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                PublishedAt = publishedAt,
                AttributesData = serverData
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
