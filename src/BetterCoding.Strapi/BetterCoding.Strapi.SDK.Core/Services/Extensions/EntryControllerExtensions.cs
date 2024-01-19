using BetterCoding.Strapi.SDK.Core.Entry;

namespace BetterCoding.Strapi.SDK.Core.Services.Extensions
{
    internal static class EntryControllerExtensions
    {
        internal static T GenerateObjectFromState<T>(this IServiceHub serviceHub, IEntryState state, string defaultClassName) where T : StrapiEntry => serviceHub.ClassController.GenerateObjectFromState<T>(state, defaultClassName, serviceHub);

        internal static T GenerateObjectFromState<T>(this IStrapiEntryClassController classController, IEntryState state, string defaultClassName, IServiceHub serviceHub) where T : StrapiEntry
        {
            T obj = (T)classController.CreateObjectWithoutData(state.EntryName ?? defaultClassName, state.Id, serviceHub);
            obj.HandleFetchResult(state);

            return obj;
        }
        public static StrapiEntry CreateObjectWithoutData(this IStrapiEntryClassController classController, string className, int id, IServiceHub serviceHub)
        {
            try
            {
                StrapiEntry result = classController.Instantiate(className, serviceHub);
                result.Id = id;
                result.IsDirty = false;
                return result.IsDirty ? throw new InvalidOperationException("An SrrapiEntry subclass default constructor must not make changes to the object that cause it to be dirty.") : result;
            }
            finally { }
        }

        internal static string GetFieldForPropertyName(this IServiceHub serviceHub, string className, string propertyName) => serviceHub.ClassController.GetPropertyMappings(className).TryGetValue(propertyName, out string fieldName) ? fieldName : fieldName;
    }
}
