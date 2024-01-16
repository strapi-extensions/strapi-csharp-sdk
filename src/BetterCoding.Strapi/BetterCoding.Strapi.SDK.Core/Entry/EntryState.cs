namespace BetterCoding.Strapi.SDK.Core.Entry
{
    public interface IEntryState : IEnumerable<KeyValuePair<string, object>>
    {
        bool IsNew { get; }
        string EntryName { get; }
        int Id { get; }
        DateTime? UpdatedAt { get; }
        DateTime? CreatedAt { get; }
        public DateTime? PublishedAt { get; }
        object this[string key] { get; }

        bool ContainsKey(string key);

        IEntryState MutatedClone(Action<MutableEntryState> func);
    }

    public class MutableEntryState : IEntryState
    {
        public bool IsNew { get; set; }
        public string EntryName { get; set; }
        public int Id { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public IDictionary<string, object> AttributesData { get; set; } = new Dictionary<string, object> { };

        public object this[string key] => AttributesData[key];

        public bool ContainsKey(string key) => AttributesData.ContainsKey(key);

        public void Apply(IEntryState other)
        {
            IsNew = other.IsNew;
            Id = other.Id;
            if (other.UpdatedAt != null)
                UpdatedAt = other.UpdatedAt;
            if (other.CreatedAt != null)
                CreatedAt = other.CreatedAt;

            foreach (KeyValuePair<string, object> pair in other)
                AttributesData[pair.Key] = pair.Value;
        }

        public IEntryState MutatedClone(Action<MutableEntryState> func)
        {
            MutableEntryState clone = MutableClone();
            func(clone);
            return clone;
        }

        protected virtual MutableEntryState MutableClone() => new MutableEntryState
        {
            IsNew = IsNew,
            EntryName = EntryName,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            PublishedAt = PublishedAt,
            AttributesData = this.ToDictionary(t => t.Key, t => t.Value)
        };

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => AttributesData.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
    }
}
