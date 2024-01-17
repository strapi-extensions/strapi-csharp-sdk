using BetterCoding.Strapi.SDK.Core.Entry;
using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core.Query
{
    public class FiltersBuilder
    {
        protected string _entryName = string.Empty;
        protected string _pluralApiId = string.Empty;

        protected IDictionary<string, object> _filters;
        public IDictionary<string, object> Filters => _filters;
        protected FiltersBuilder Instance => this;

        public IServiceHub Services { get; internal set; }

        public FiltersBuilder(IServiceHub serviceHub = default)
        {
            _filters = new Dictionary<string, object>();
            Services = StrapiClient.GetClient().Services;
        }

        public virtual FiltersBuilder EntryName(string entryName)
        {
            _entryName = entryName;
            return Instance;
        }

        public virtual FiltersBuilder PluralApiId(string pluralApiId)
        {
            _pluralApiId = pluralApiId;
            return Instance;
        }

        public virtual FiltersBuilder MergerFilter(IDictionary<string, object> filter)
        {
            _filters = new Dictionary<string, object>(MergeFilterClauses(filter));
            return Instance;
        }

        IDictionary<string, object> MergeFilterClauses(IDictionary<string, object> filter)
        {
            if (Filters is null)
            {
                return filter;
            }

            Dictionary<string, object> newWhere = new Dictionary<string, object>(Filters);
            foreach (KeyValuePair<string, object> pair in filter)
            {
                if (newWhere.ContainsKey(pair.Key))
                {
                    if (!(newWhere[pair.Key] is IDictionary<string, object> oldCondition) || !(pair.Value is IDictionary<string, object> condition))
                    {
                        throw new ArgumentException("more than one filter clause for the given key provided.");
                    }

                    Dictionary<string, object> newCondition = new Dictionary<string, object>(oldCondition);
                    foreach (KeyValuePair<string, object> conditionPair in condition)
                    {
                        if (newCondition.ContainsKey(conditionPair.Key))
                        {
                            throw new ArgumentException("more than one condition for the given key provided.");
                        }

                        newCondition[conditionPair.Key] = conditionPair.Value;
                    }

                    newWhere[pair.Key] = newCondition;
                }
                else
                {
                    newWhere[pair.Key] = pair.Value;
                }
            }
            return newWhere;
        }

        public virtual FiltersBuilder BuildFilters(string op, object value, params string[] deepFields)
        {
            if (!deepFields.Any()) throw new KeyNotFoundException("no deepFields found.");
            var reversed = deepFields.Reverse();

            var filter = new Dictionary<string, object> { };
            foreach (var field in reversed)
            {
                if (!filter.Any())
                    filter[field] = new Dictionary<string, object> { { op, value } };
                else
                {
                    var newFilter = new Dictionary<string, object> { { field, filter } };
                    filter = newFilter;
                }
            }
            return MergerFilter(filter);
        }

        public virtual FiltersBuilder EqualsTo(string field, object value)
        {
            return BuildFilters("$eq", value, field);
        }

        public virtual FiltersBuilder DeepEqualsTo(string relationField, string deepField, object value)
        {
            return BuildFilters("$eq", value, relationField, deepField);
        }

        public virtual FiltersBuilder ContainedIn<TIn>(string field, IEnumerable<TIn> values)
        {
            return BuildFilters("$in", values.ToList(), field);
        }

        public virtual FiltersBuilder LaterThan(string field, DateTime utc)
        {
            return BuildFilters("$lt", utc, field);
        }

        public virtual FiltersBuilder EarlierThan(string field, DateTime utc)
        {
            return BuildFilters("$gt", utc, field);
        }

        public virtual string Encode()
        {
            var filters = Services.QueryStringEncoder.Encode(new Dictionary<string, object>
            {
                { "filters", _filters }
            });
            return filters;
        }

        public virtual async Task<IEnumerable<StrapiEntry>> FindAsync()
        {
            var filters = Encode();

            var result = await StrapiClient.GetClient().GetREST().Filtering(_entryName, _pluralApiId, filters);

            return result;
        }
    }
}
