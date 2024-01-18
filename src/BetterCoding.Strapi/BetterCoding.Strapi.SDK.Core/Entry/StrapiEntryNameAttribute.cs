namespace BetterCoding.Strapi.SDK.Core.Entry
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class StrapiEntryNameAttribute : Attribute
    {
        public StrapiEntryNameAttribute(string entryName) => EntryName = entryName;

        public string EntryName { get; private set; }
    }
}
