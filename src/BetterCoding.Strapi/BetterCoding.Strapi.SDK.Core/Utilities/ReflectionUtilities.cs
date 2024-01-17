using BetterCoding.Strapi.SDK.Core.Entry;
using System.Reflection;

namespace BetterCoding.Strapi.SDK.Core.Utilities
{
    public static class ReflectionUtilities
    {
        public static IEnumerable<ConstructorInfo> GetInstanceConstructors(this Type type) => type.GetTypeInfo().DeclaredConstructors.Where(constructor => (constructor.Attributes & MethodAttributes.Static) == 0);

        public static ConstructorInfo FindConstructor(this Type self, params Type[] parameterTypes) => self.GetConstructors().Where(constructor => constructor.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(parameterTypes)).SingleOrDefault();
        public static bool CheckWrappedWithNullable(this Type type) => type.IsConstructedGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));

        public static string GetStrapiEntryName(this Type type) => type.GetCustomAttribute<StrapiEntryNameAttribute>()?.EntryName;
    }
}
