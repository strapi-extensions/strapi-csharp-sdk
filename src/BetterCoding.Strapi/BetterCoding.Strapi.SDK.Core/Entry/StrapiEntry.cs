using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core.Entry
{
    public class StrapiEntry : IEnumerable<KeyValuePair<string, object>>
    {
        internal static string AutoClassName { get; } = "_Automatic";
        public DateTime? CreatedAt => State.CreatedAt;
        public DateTime? UpdatedAt => State.UpdatedAt;
        public DateTime? PublishedAt => State.PublishedAt;
        public IServiceHub Services { get; set; }
        public string ModelName => State.EntryName;

        internal IEntryState State { get; private set; }

        internal object Mutex { get; } = new object { };


        public StrapiEntry(string entryName, IServiceHub serviceHub = default)
        {
            State = new MutableEntryState { EntryName = entryName };
        }

        internal IDictionary<string, object> EstimatedData { get; } = new Dictionary<string, object> { };
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            lock (Mutex)
            {
                return EstimatedData.GetEnumerator();
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (Mutex)
            {
                return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
            }
        }
        public ICollection<string> Keys
        {
            get
            {
                lock (Mutex)
                {
                    return EstimatedData.Keys;
                }
            }
        }

        public virtual void HandleFetchResult(IEntryState serverState)
        {
            lock (Mutex)
            {
                MergeFromServer(serverState);
            }
        }

        internal bool Fetched { get; set; }

        internal virtual void MergeFromServer(IEntryState serverState)
        {
            Dictionary<string, object> newServerData = serverState.ToDictionary(t => t.Key, t => t.Value);

            lock (Mutex)
            {
                if (serverState.Id != 0)
                {
                    Fetched = true;
                }

                MutateState(mutableClone => mutableClone.Apply(serverState.MutatedClone(mutableClone => mutableClone.AttributesData = newServerData)));
            }
        }

        internal void MutateState(Action<MutableEntryState> mutator)
        {
            lock (Mutex)
            {
                State = State.MutatedClone(mutator);
                RebuildEstimatedData();
            }
        }

        internal void RebuildEstimatedData()
        {
            lock (Mutex)
            {
                EstimatedData.Clear();

                foreach (KeyValuePair<string, object> item in State)
                {
                    EstimatedData.Add(item);
                }
            }
        }
    }
}
