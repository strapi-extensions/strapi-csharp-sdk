namespace BetterCoding.Strapi.SDK.Core.Entry
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class StrapiEntryNameAttribute : Attribute
    {
        /// <summary>
        /// Constructs a new ParseClassName attribute.
        /// </summary>
        /// <param name="entryName">The class name to associate with the ParseObject subclass.</param>
        public StrapiEntryNameAttribute(string entryName) => EntryName = entryName;

        /// <summary>
        /// Gets the class name to associate with the ParseObject subclass.
        /// </summary>
        public string EntryName { get; private set; }
    }
}
