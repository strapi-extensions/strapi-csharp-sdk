using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core.Entry.FieldOperations
{
    public class StrapiSetOperation: IStrapiFieldOperation
    {
        public StrapiSetOperation(object value) => Value = value;

        public object Encode(IServiceHub serviceHub) => serviceHub.AttributeEncoder.Encode(Value, serviceHub);

        public IStrapiFieldOperation MergeWithPrevious(IStrapiFieldOperation previous) => this;

        public object Apply(object oldValue, string key) => Value;

        public object Value { get; private set; }
    }
}
