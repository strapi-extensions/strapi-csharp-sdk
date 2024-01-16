using BetterCoding.Strapi.SDK.Core.Query;
using System.Text.Encodings.Web;
using System.Web;

namespace BetterCoding.Strapi.SDK.Core.Tests
{
    public class QueryBuilderTests
    {
        [Fact]
        public void QueryString_ShouldContainsArray_Test()
        {
            var builder = new StrapiClient().GetQueryBuilder().ContainedIn("id", new List<object> { 1, 2, 3 });
            
        }
    }
}