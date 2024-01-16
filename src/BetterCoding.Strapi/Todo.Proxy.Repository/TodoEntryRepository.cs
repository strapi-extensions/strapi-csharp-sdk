using BetterCoding.Strapi.SDK.Core;
using Todo.Proxy.Contracts;

namespace Todo.Proxy.Repository
{
    public class TodoEntryRepository
    {
        public async Task<List<TodoEntry>> GetByWhoCreated(int userId)
        {
            var serverConfiguration = new StrapiServerConfiguration()
            {
                APIToken = "aa1f50223e80dcf00912a3c827bbd6012a5065620fafe397c0ba3af464e617876590f5458ba72580709285c48790940c17127c498f3dbd67e7854393cbdff6433110cd44080af25ad9c0564e8483ad4351677998959d959e275e124f544e415c0eeb02430c138dcb891e694641cb3c21720f197403053ea5019728140172f4da",
                ServerURI = "http://localhost:1337",
            };

            var strapiClient = new StrapiClient(serverConfiguration);

            var todos = await StrapiClient.Instance.GetQueryBuilder()
                 .EntryName("todo").PluralApiId("todos").DeepEqualsTo("whoCreated", "id", userId).FindAsync();

            return new List<TodoEntry> { };
        }
    }
}
