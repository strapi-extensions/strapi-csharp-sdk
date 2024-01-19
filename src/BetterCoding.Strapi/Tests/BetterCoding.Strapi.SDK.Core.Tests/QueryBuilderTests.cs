using BetterCoding.Strapi.SDK.Core.Query;
using BetterCoding.Strapi.SDK.Core.Server;
using System.Text.Encodings.Web;
using System.Web;

namespace BetterCoding.Strapi.SDK.Core.Tests
{
    public class QueryBuilderTests
    {
        [Fact]
        public void BuildFilters_Should_EqualsDeepFilters_Test()
        {
            var serverConfiguration = new StrapiServerConfiguration()
            {
                APIToken = "aa1f50223e80dcf00912a3c827bbd6012a5065620fafe397c0ba3af464e617876590f5458ba72580709285c48790940c17127c498f3dbd67e7854393cbdff6433110cd44080af25ad9c0564e8483ad4351677998959d959e275e124f544e415c0eeb02430c138dcb891e694641cb3c21720f197403053ea5019728140172f4da",
                ServerURI = "http://localhost:1337",
                Alias = "development",
                IsDefault = true,
            };

            StrapiClient.AddServer(serverConfiguration);

            var actual = new StrapiClient().GetFiltersBuilder().BuildFilters("$eq", 1, "whoCreated", "id").Encode();

            Assert.Equal("filters[whoCreated][id][$eq]=1", actual);
        }
    }
}