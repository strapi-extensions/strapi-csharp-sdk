using BetterCoding.Strapi.SDK.Core.Services;

namespace BetterCoding.Strapi.SDK.Core.Entry
{
    public interface IStrapiEntryClassController
    {
        string GetClassName(Type type);

        Type GetType(string className);

        bool GetClassMatch(string className, Type type);

        void AddValid(Type type);

        void RemoveClass(Type type);

        void AddRegisterHook(Type type, Action action);

        StrapiEntry Instantiate(string className, IServiceHub serviceHub);

        IDictionary<string, string> GetPropertyMappings(string className);

        void AddIntrinsic();
    }
}
