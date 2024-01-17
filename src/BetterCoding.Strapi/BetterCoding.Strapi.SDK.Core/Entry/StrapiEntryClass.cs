using BetterCoding.Strapi.SDK.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetterCoding.Strapi.SDK.Core.Entry
{
    public class StrapiEntryClass
    {
        public StrapiEntryClass(Type type, ConstructorInfo constructor)
        {
            TypeInfo = type.GetTypeInfo();
            DeclaredName = TypeInfo.GetStrapiEntryName();
            Constructor = Constructor = constructor;
            PropertyMappings = type.GetProperties().Select(property => (Property: property, FieldNameAttribute: property.GetCustomAttribute<StrapiFieldNameAttribute>(true))).Where(set => set.FieldNameAttribute is { }).ToDictionary(set => set.Property.Name, set => set.FieldNameAttribute.FieldName);
        }

        public TypeInfo TypeInfo { get; }

        public string DeclaredName { get; }

        public IDictionary<string, string> PropertyMappings { get; }

        public StrapiEntry Instantiate() => Constructor.Invoke(default) as StrapiEntry;

        ConstructorInfo Constructor { get; }
    }
}
