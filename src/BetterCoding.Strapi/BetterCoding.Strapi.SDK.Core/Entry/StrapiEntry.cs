using BetterCoding.Strapi.SDK.Core.Entry.FieldOperations;
using BetterCoding.Strapi.SDK.Core.Services;
using BetterCoding.Strapi.SDK.Core.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BetterCoding.Strapi.SDK.Core.Entry
{
    public class StrapiEntry : IEnumerable<KeyValuePair<string, object>>
    {
        internal static string AutoClassName { get; } = "_Automatic";

        public int Id => State.Id;
        public DateTime? CreatedAt => State.CreatedAt;
        public DateTime? UpdatedAt => State.UpdatedAt;
        public DateTime? PublishedAt => State.PublishedAt;
        public IServiceHub Services { get; set; }
        public string EntryName => State.EntryName;

        internal IEntryState State { get; private set; }

        internal object Mutex { get; } = new object { };

        public T Get<T>(string key) => Conversion.To<T>(this[key]);
        public StrapiEntry(string entryName, IServiceHub serviceHub = default)
        {
            State = new MutableEntryState { EntryName = entryName };
        }
        void CheckGetAccess(string key)
        {
            lock (Mutex)
            {
                if (!CheckIsDataAvailable(key))
                {
                    throw new InvalidOperationException("StrapiEntry has no data for this key. Call FetchIfNeededAsync() to get the data.");
                }
            }
        }
        public StrapiEntry Bind(IServiceHub serviceHub) => (Instance: this, Services = serviceHub).Instance;
        public bool IsDataAvailable
        {
            get
            {
                lock (Mutex)
                {
                    return Fetched;
                }
            }
        }
        bool CheckIsDataAvailable(string key)
        {
            lock (Mutex)
            {
                return IsDataAvailable || EstimatedData.ContainsKey(key);
            }
        }

        virtual public object this[string key]
        {
            get
            {
                lock (Mutex)
                {
                    CheckGetAccess(key);
                    object value = EstimatedData[key];

                    // A relation may be deserialized without a parent or key. Either way,
                    // make sure it's consistent.

                    //if (value is ParseRelationBase relation)
                    //{
                    //    relation.EnsureParentAndKey(this, key);
                    //}

                    return value;
                }
            }
            set
            {
                lock (Mutex)
                {
                    CheckKeyIsMutable(key);
                    Set(key, value);
                }
            }
        }
        public void Set(string key, object value)
        {
            lock (Mutex)
            {
                OnSettingValue(ref key, ref value);

                if (!Services.AttributeEncoder.Validate(value))
                {
                    throw new ArgumentException("Invalid type for value: " + value.GetType().ToString());
                }

                PerformOperation(key, new StrapiSetOperation(value));
            }
        }
        LinkedList<IDictionary<string, IStrapiFieldOperation>> OperationSetQueue { get; } = new LinkedList<IDictionary<string, IStrapiFieldOperation>>();
        public IDictionary<string, IStrapiFieldOperation> CurrentOperations
        {
            get
            {
                lock (Mutex)
                {
                    return OperationSetQueue.Last.Value;
                }
            }
        }
        bool Dirty { get; set; }
        internal bool CheckIsDirty(bool considerChildren)
        {
            lock (Mutex)
            {
                return Dirty || CurrentOperations.Count > 0;
            }
        }
        public bool IsDirty
        {
            get
            {
                lock (Mutex)
                {
                    return CheckIsDirty(true);
                }
            }
            internal set
            {
                lock (Mutex)
                {
                    Dirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChangedHandler.Invoke(this, new PropertyChangedEventArgs(propertyName));
        SynchronizedEventHandler<PropertyChangedEventArgs> PropertyChangedHandler { get; } = new SynchronizedEventHandler<PropertyChangedEventArgs>();
        internal void PerformOperation(string key, IStrapiFieldOperation operation)
        {
            lock (Mutex)
            {
                EstimatedData.TryGetValue(key, out object oldValue);
                object newValue = operation.Apply(oldValue, key);

                if (newValue != StrapiDeleteOperation.Token)
                {
                    EstimatedData[key] = newValue;
                }
                else
                {
                    EstimatedData.Remove(key);
                }

                bool wasDirty = CurrentOperations.Count > 0;
                CurrentOperations.TryGetValue(key, out IStrapiFieldOperation oldOperation);
                IStrapiFieldOperation newOperation = operation.MergeWithPrevious(oldOperation);
                CurrentOperations[key] = newOperation;

                if (!wasDirty)
                {
                    OnPropertyChanged(nameof(IsDirty));
                }

                OnFieldsChanged(new[] { key });
            }
        }
        protected void OnFieldsChanged(IEnumerable<string> fields)
        {
            IDictionary<string, string> mappings = Services.ClassController.GetPropertyMappings(EntryName);

            foreach (string property in mappings is { } ? fields is { } ? from mapping in mappings join field in fields on mapping.Value equals field select mapping.Key : mappings.Keys : Enumerable.Empty<string>())
            {
                OnPropertyChanged(property);
            }

            OnPropertyChanged("Item[]");
        }
        internal virtual void OnSettingValue(ref string key, ref object value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
        }

        void CheckKeyIsMutable(string key)
        {
            if (!CheckKeyMutable(key))
            {
                throw new InvalidOperationException($@"Cannot change the ""{key}"" property of a ""{EntryName}"" object.");
            }
        }
        protected virtual bool CheckKeyMutable(string key) => true;
        public bool ContainsKey(string key)
        {
            lock (Mutex)
            {
                return EstimatedData.ContainsKey(key);
            }
        }
        public bool TryGetValue<T>(string key, out T result)
        {
            lock (Mutex)
            {
                if (ContainsKey(key))
                {
                    try
                    {
                        T temp = Conversion.To<T>(this[key]);
                        result = temp;
                        return true;
                    }
                    catch
                    {
                        result = default;
                        return false;
                    }
                }

                result = default;
                return false;
            }
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
