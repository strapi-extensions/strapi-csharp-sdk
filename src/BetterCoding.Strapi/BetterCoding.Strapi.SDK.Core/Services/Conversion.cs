using BetterCoding.Strapi.SDK.Core.Entry;
using BetterCoding.Strapi.SDK.Core.Utilities;
using System.Collections;
using System.Reflection;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    /// <summary>
    /// A reimplementation of Xamarin's PreserveAttribute.
    /// This allows us to support AOT and linking for Xamarin platforms.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }

    [AttributeUsage(AttributeTargets.All)]
    internal class LinkerSafeAttribute : Attribute
    {
        public LinkerSafeAttribute() { }
    }

    [Preserve(AllMembers = true, Conditional = false)]
    public class FlexibleDictionaryWrapper<TOut, TIn> : IDictionary<string, TOut>
    {
        private readonly IDictionary<string, TIn> toWrap;
        public FlexibleDictionaryWrapper(IDictionary<string, TIn> toWrap) => this.toWrap = toWrap;

        public void Add(string key, TOut value) => toWrap.Add(key, (TIn)Conversion.ConvertTo<TIn>(value));

        public bool ContainsKey(string key) => toWrap.ContainsKey(key);

        public ICollection<string> Keys => toWrap.Keys;

        public bool Remove(string key) => toWrap.Remove(key);

        public bool TryGetValue(string key, out TOut value)
        {
            bool result = toWrap.TryGetValue(key, out TIn outValue);
            value = (TOut)Conversion.ConvertTo<TOut>(outValue);
            return result;
        }

        public ICollection<TOut> Values => toWrap.Values
                    .Select(item => (TOut)Conversion.ConvertTo<TOut>(item)).ToList();

        public TOut this[string key]
        {
            get => (TOut)Conversion.ConvertTo<TOut>(toWrap[key]);
            set => toWrap[key] = (TIn)Conversion.ConvertTo<TIn>(value);
        }

        public void Add(KeyValuePair<string, TOut> item) => toWrap.Add(new KeyValuePair<string, TIn>(item.Key,
                (TIn)Conversion.ConvertTo<TIn>(item.Value)));

        public void Clear() => toWrap.Clear();

        public bool Contains(KeyValuePair<string, TOut> item) => toWrap.Contains(new KeyValuePair<string, TIn>(item.Key,
                (TIn)Conversion.ConvertTo<TIn>(item.Value)));

        public void CopyTo(KeyValuePair<string, TOut>[] array, int arrayIndex)
        {
            IEnumerable<KeyValuePair<string, TOut>> converted = from pair in toWrap
                                                                select new KeyValuePair<string, TOut>(pair.Key,
                                                                    (TOut)Conversion.ConvertTo<TOut>(pair.Value));
            converted.ToList().CopyTo(array, arrayIndex);
        }

        public int Count => toWrap.Count;

        public bool IsReadOnly => toWrap.IsReadOnly;

        public bool Remove(KeyValuePair<string, TOut> item) => toWrap.Remove(new KeyValuePair<string, TIn>(item.Key,
                (TIn)Conversion.ConvertTo<TIn>(item.Value)));

        public IEnumerator<KeyValuePair<string, TOut>> GetEnumerator()
        {
            foreach (KeyValuePair<string, TIn> pair in toWrap)
                yield return new KeyValuePair<string, TOut>(pair.Key,
                    (TOut)Conversion.ConvertTo<TOut>(pair.Value));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class FlexibleListWrapper<TOut, TIn> : IList<TOut>
    {
        private IList<TIn> toWrap;
        public FlexibleListWrapper(IList<TIn> toWrap) => this.toWrap = toWrap;

        public int IndexOf(TOut item) => toWrap.IndexOf((TIn)Conversion.ConvertTo<TIn>(item));

        public void Insert(int index, TOut item) => toWrap.Insert(index, (TIn)Conversion.ConvertTo<TIn>(item));

        public void RemoveAt(int index) => toWrap.RemoveAt(index);

        public TOut this[int index]
        {
            get => (TOut)Conversion.ConvertTo<TOut>(toWrap[index]);
            set => toWrap[index] = (TIn)Conversion.ConvertTo<TIn>(value);
        }

        public void Add(TOut item) => toWrap.Add((TIn)Conversion.ConvertTo<TIn>(item));

        public void Clear() => toWrap.Clear();

        public bool Contains(TOut item) => toWrap.Contains((TIn)Conversion.ConvertTo<TIn>(item));

        public void CopyTo(TOut[] array, int arrayIndex) => toWrap.Select(item => (TOut)Conversion.ConvertTo<TOut>(item))
                .ToList().CopyTo(array, arrayIndex);

        public int Count => toWrap.Count;

        public bool IsReadOnly => toWrap.IsReadOnly;

        public bool Remove(TOut item) => toWrap.Remove((TIn)Conversion.ConvertTo<TIn>(item));

        public IEnumerator<TOut> GetEnumerator()
        {
            foreach (object item in (IEnumerable)toWrap)
                yield return (TOut)Conversion.ConvertTo<TOut>(item);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class Conversion
    {
        /// <summary>
        /// Converts a value to the requested type -- coercing primitives to
        /// the desired type, wrapping lists and dictionaries appropriately,
        /// or else returning null.
        ///
        /// This should be used on any containers that might be coming from a
        /// user to normalize the collection types. Collection types coming from
        /// JSON deserialization can be safely assumed to be lists or dictionaries of
        /// objects.
        /// </summary>
        public static T As<T>(object value) where T : class => ConvertTo<T>(value) as T;

        /// <summary>
        /// Converts a value to the requested type -- coercing primitives to
        /// the desired type, wrapping lists and dictionaries appropriately,
        /// or else throwing an exception.
        ///
        /// This should be used on any containers that might be coming from a
        /// user to normalize the collection types. Collection types coming from
        /// JSON deserialization can be safely assumed to be lists or dictionaries of
        /// objects.
        /// </summary>
        public static T To<T>(object value) => (T)ConvertTo<T>(value);

        /// <summary>
        /// Converts a value to the requested type -- coercing primitives to
        /// the desired type, wrapping lists and dictionaries appropriately,
        /// or else passing the object along to the caller unchanged.
        ///
        /// This should be used on any containers that might be coming from a
        /// user to normalize the collection types. Collection types coming from
        /// JSON deserialization can be safely assumed to be lists or dictionaries of
        /// objects.
        /// </summary>
        internal static object ConvertTo<T>(object value)
        {
            if (value is T || value == null)
                return value;

            if (typeof(T).IsPrimitive)
                return (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);

            if (typeof(T).IsConstructedGenericType)
            {
                // Add lifting for nullables. Only supports conversions between primitives.

                if (typeof(T).CheckWrappedWithNullable() && typeof(T).GenericTypeArguments[0] is { IsPrimitive: true } innerType)
                    return (T)Convert.ChangeType(value, innerType, System.Globalization.CultureInfo.InvariantCulture);

                if (GetInterfaceType(value.GetType(), typeof(IList<>)) is { } listType && typeof(T).GetGenericTypeDefinition() == typeof(IList<>))
                    return Activator.CreateInstance(typeof(FlexibleListWrapper<,>).MakeGenericType(typeof(T).GenericTypeArguments[0], listType.GenericTypeArguments[0]), value);

                if (GetInterfaceType(value.GetType(), typeof(IDictionary<,>)) is { } dictType && typeof(T).GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return Activator.CreateInstance(typeof(FlexibleDictionaryWrapper<,>).MakeGenericType(typeof(T).GenericTypeArguments[1], dictType.GenericTypeArguments[1]), value);
            }

            return value;
        }

        /// <summary>
        /// Holds a dictionary that maps a cache of interface types for related concrete types.
        /// The lookup is slow the first time for each type because it has to enumerate all interface
        /// on the object type, but made fast by the cache.
        ///
        /// The map is:
        ///    (object type, generic interface type) => constructed generic type
        /// </summary>
        static Dictionary<Tuple<Type, Type>, Type> InterfaceLookupCache { get; } = new Dictionary<Tuple<Type, Type>, Type>();

        static Type GetInterfaceType(Type objType, Type genericInterfaceType)
        {
            Tuple<Type, Type> cacheKey = new Tuple<Type, Type>(objType, genericInterfaceType);

            if (InterfaceLookupCache.ContainsKey(cacheKey))
                return InterfaceLookupCache[cacheKey];

            foreach (Type type in objType.GetInterfaces())
                if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == genericInterfaceType)
                    return InterfaceLookupCache[cacheKey] = type;

            return default;
        }
    }
}
