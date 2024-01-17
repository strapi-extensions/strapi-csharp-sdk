using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core.Entry.FieldOperations
{
    public class StrapiDeleteOperation: IStrapiFieldOperation
    {
        internal static object Token { get; } = new object { };

        public static StrapiDeleteOperation Instance { get; } = new StrapiDeleteOperation { };

        private StrapiDeleteOperation() { }

        public object Encode(IServiceHub serviceHub) => new Dictionary<string, object> { ["__op"] = "Delete" };

        public IStrapiFieldOperation MergeWithPrevious(IStrapiFieldOperation previous) => this;

        public object Apply(object oldValue, string key) => Token;
    }
}
