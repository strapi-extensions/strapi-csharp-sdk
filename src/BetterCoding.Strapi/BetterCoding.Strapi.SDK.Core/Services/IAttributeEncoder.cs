using BetterCoding.Strapi.SDK.Core.Entry;
using BetterCoding.Strapi.SDK.Core.Entry.FieldOperations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCoding.Strapi.SDK.Core.Services
{
    public interface IAttributeEncoder
    {
        bool Validate(object value);
        object Encode(object value, IServiceHub serviceHub);
    }

    public class AttributeEncoder : IAttributeEncoder 
    {
        public bool Validate(object value)  =>
            value is null || value.GetType().IsPrimitive 
            || value is string || value is StrapiEntry
            || value is DateTime || value is byte[] 
            || Conversion.As<IDictionary<string, object>>(value) is { } || Conversion.As<IList<object>>(value) is { };

        public object Encode(object value, IServiceHub serviceHub) => value switch
        {
            StrapiEntry { } entity => EncodeObject(entity),
            { } when Conversion.As<IDictionary<string, object>>(value) is { } dictionary => dictionary.ToDictionary(pair => pair.Key, pair => Encode(pair.Value, serviceHub)),
            { } when Conversion.As<IList<object>>(value) is { } list => EncodeList(list, serviceHub),
            IStrapiFieldOperation { } fieldOperation => fieldOperation.Encode(serviceHub),
            _ => value
        };
        object EncodeObject(StrapiEntry value) 
        {
            return value;
        }
        object EncodeList(IList<object> list, IServiceHub serviceHub)
        {
            List<object> encoded = new List<object> { };

            if (list.GetType().IsArray)
            {
                list = new List<object>(list);
            }

            foreach (object item in list)
            {
                if (!Validate(item))
                {
                    throw new ArgumentException("Invalid type for value in an array");
                }

                encoded.Add(Encode(item, serviceHub));
            }

            return encoded;
        }
    }
}
