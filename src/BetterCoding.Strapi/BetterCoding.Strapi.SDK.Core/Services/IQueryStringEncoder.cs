namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IQueryStringEncoder
    {
        string Encode(object obj, params string[] keyRoadMap);
        string Encode(IDictionary<string, object> dictionary, params string[] keyRoadMap);
    }

    public class QueryStringEncoder : IQueryStringEncoder
    {
        public string Encode(object obj, params string[] keyRoadMap)
        {
            if (keyRoadMap.Length == 1) return $"[{keyRoadMap[0]}]={obj}";

            var removedRootKey = keyRoadMap.Skip(1).ToList();
            var encodedKey = string.Join("", removedRootKey.Select(x => string.Format("[{0}]", x)).ToList());

            return $"{keyRoadMap[0]}{encodedKey}={obj}";
        }

        public string Encode(IList<object> list, params string[] keyRoadMap)
        {
            var results = new List<string>();
            var keyList = keyRoadMap.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var newKeyList = new List<string>(keyList);
                newKeyList.Add(i.ToString());
                var encoded = Encode(item, newKeyList.ToArray());
                results.Add(encoded);
            }

            return string.Join('&', results);
        }

        public string Encode(IDictionary<string, object> dictionary, params string[] keyRoadMap)
        {
            if (dictionary == null)
                throw new ArgumentNullException();
            if (dictionary.Count == 0)
                return "{}";
            var list = dictionary.ToList().Select(pair => Encode(pair, keyRoadMap)).ToList();
            return string.Join('&', list);
        }

        public string Encode(KeyValuePair<string, object> pair, params string[] keyRoadMap)
        {
            var keyList = keyRoadMap.ToList();
            keyList.Add(pair.Key);

            if (pair.Value is IDictionary<string, object> dictionary)
            {
                return Encode(dictionary, keyList.ToArray());
            }
            else if (pair.Value is KeyValuePair<string, object> subPair)
            {
                return Encode(subPair, keyList.ToArray());
            }
            else if (pair.Value is IList<object> list)
            {
                return Encode(list, keyList.ToArray());
            }

            return Encode(pair.Value, keyList.ToArray());
        }
    }
}
