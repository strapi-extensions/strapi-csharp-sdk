using BetterCoding.Strapi.SDK.Core.Entry;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IUserStateCoder
    {
        IEntryState Decode(IDictionary<string, object> data, IDataDecoder decoder, IServiceHub serviceHub);
    }

    public class LocalUserStateCoder : EntryStateCoder, IUserStateCoder
    {
        public override IEntryState Decode(IDictionary<string, object> data, IDataDecoder decoder, IServiceHub serviceHub)
        {
            IDictionary<string, object> serverData = new Dictionary<string, object> { },
               mutableData = new Dictionary<string, object>(data);

            int id = Extract(mutableData, "id", (obj) => int.Parse(obj.ToString()));
            string jwt = Extract(mutableData, "jwt", s => s.ToString());
            serverData["jwt"] = jwt;
            var user = Extract<Dictionary<string, object>>(mutableData, "user", obj => obj as Dictionary<string, object>);
            DateTime? createdAt = Extract<DateTime?>(user, "createdAt", (obj) => DataDecoder.ParseDate(obj as string)),
                updatedAt = Extract<DateTime?>(user, "updatedAt", (obj) => DataDecoder.ParseDate(obj as string));

            if (createdAt != null && updatedAt == null)
            {
                updatedAt = createdAt;
            }

            foreach (KeyValuePair<string, object> pair in user)
            {
                serverData[pair.Key] = decoder.Decode(pair.Value, serviceHub);
            }

            return new MutableEntryState
            {
                Id = id,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                AttributesData = serverData
            };
        }
    }
}
